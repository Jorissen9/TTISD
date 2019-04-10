import ctypes as ct
import serial


def to_bytes(*args, type=ct.c_uint8):
    b = b''

    for arg in args:
        b += bytes(type(arg))

    return b


def to_int(arg, byteorder="big", signed=True):
    return int.from_bytes(arg, byteorder=byteorder, signed=signed)


def to_ints_iterate(arg, size=2, byteorder="big", signed=True):
    if len(arg) <= size:
        yield to_int(arg, byteorder, signed)
    else:
        yield to_int(arg[0:size], byteorder, signed)
        yield from to_ints_iterate(arg[size:], size, byteorder, signed)


def limits(c_int_type):
    signed = c_int_type(-1).value < c_int_type(0).value
    bit_size = ct.sizeof(c_int_type) * 8
    signed_limit = 2 ** (bit_size - 1)
    return (-signed_limit, signed_limit - 1) if signed else (0, 2 * signed_limit - 1)


def clamp(n, smallest, largest):
    return max(smallest, min(n, largest))


def make_interpolater(left_min, left_max, right_min, right_max):
    # Figure out how 'wide' each range is
    leftSpan = left_max - left_min
    rightSpan = right_max - right_min

    # Compute the scale factor between left and right values
    scaleFactor = float(rightSpan) / float(leftSpan)

    # create interpolation function using pre-calculated scaleFactor
    def interp_fn(value):
        return clamp(right_min + (value-left_min)*scaleFactor, right_min, right_max)

    return interp_fn


def make_int16_interpolater(left_min, left_max, right_min, right_max, offset=0):
    interp_fn = make_interpolater(left_min, left_max, right_min, right_max)

    def int_interp_fn(value):
        return int(interp_fn(value + offset))

    return int_interp_fn


translate_1000s_to_int16 = make_int16_interpolater(-1000, 1000, *limits(ct.c_int16))   # (-1000, 1000) -> (-32768, 32767)
translate_1000u_to_int16 = make_int16_interpolater(0, 1000, 0, limits(ct.c_int16)[1])  # (    0, 1000) -> (     0, 32767)

# Normally input would be between 0 and 1000, but full range is limited by enclosure.
# The values below were determined manually.
translate_stick_X = make_int16_interpolater(315, 931, *limits(ct.c_int16), offset=87)
translate_stick_Y = make_int16_interpolater(206, 670, *limits(ct.c_int16), offset=40)


###############################################################################
# Payload messages
###############################################################################

class RotationMessage:
    MASK_PITCH = 1
    MASK_ROLL  = 2
    MASK_YAW   = 4

    def __init__(self, pitch=0, roll=0, yaw=0, mask=0):
        self.pitch = pitch  # Vessel pitch
        self.roll  = roll   # Vessel roll
        self.yaw   = yaw    # Vessel yaw

        # The mask indicates which elements are intentionally set.
        #   Unset elements should be ignored. It should be one or more of:
        #
        #       - 1: pitch
        #       - 2: roll
        #       - 4: yaw
        self.mask = mask

    def reset(self):
        self.mask = 0

    def setPitch(self, pitch):
        self.pitch = pitch
        self.mask |= RotationMessage.MASK_PITCH

    def setRoll(self, roll):
        self.roll = roll
        self.mask |= RotationMessage.MASK_ROLL

    def setYaw(self, yaw):
        self.yaw = yaw
        self.mask |= RotationMessage.MASK_YAW

    def bytes(self):
        return to_bytes(self.pitch, self.roll, self.yaw, type=ct.c_int16) \
             + to_bytes(self.mask)


###############################################################################
# Kerbal Simpit Message Types
###############################################################################

# Common packets.
# These packet types are used for both inbound and outbound messages.
class CommonPackets:
    SYNC_MESSAGE      = 0  # Sync message. Used for handshaking.
    ECHO_REQ_MESSAGE  = 1  # Echo request. Either end can send this,
                           # and an echo response is expected.
    ECHO_RESP_MESSAGE = 2  # Echo response. Sent in reply to an echo request.


# Inbound packets.
# These packet types are used for packets going from devices to the game.
class InboundPackets:
    # Register to receive messages on a given channel.
    REGISTER_MESSAGE = 8

    # Deregister, indicate that no further messages
    # for the given channel should be sent.
    DEREGISTER_MESSAGE = 9

    # Custom action packets activate and deactivate custom action groups
    # Activate the given Custom Action Group(s).
    CAGACTIVATE_MESSAGE = 10

    # Deactivate the given Custom Action Group(s).
    CAGDEACTIVATE_MESSAGE = 11

    # Toggle the given Custom Action Group(s) (Active CAGs will deactivate,
    # inactive CAGs will activate).
    CAGTOGGLE_MESSAGE = 12

    # Activate the given standard Action Group(s).
    #     Note that *every request* to activate the Stage action group will
    #     result in the next stage being activated.
    #     For all other action groups, multiple activate requests will have
    #     no effect.
    AGACTIVATE_MESSAGE = 13

    # Deactivate the given standard Action Group(s).
    AGDEACTIVATE_MESSAGE = 14

    # Toggle the given standard Action Group(s).
    AGTOGGLE_MESSAGE = 15

    # Send vessel rotation commands.
    ROTATION_MESSAGE = 16

    # Send vessel translation commands
    TRANSLATION_MESSAGE = 17

    # Send wheel steering/throttle commands.
    WHEEL_MESSAGE = 18

    # Send vessel throttle commands
    THROTTLE_MESSAGE = 19

    # Send SAS mode commands.
    #    The payload should be a single byte, possible SAS modes are listed
    #    in the AutopilotMode enum. */
    SAS_MODE_MESSAGE = 20


###############################################################################
# Kerbal Simpit main
###############################################################################

class Communication:
    def __init__(self, port, baudrate=115200):
        self.send_str = ""

        self.serial = serial.Serial(port=port, baudrate=baudrate)

    def reset(self):
        self.send_str = ""

    def resetb(self):
        self.send_str = b""

    def append(self, string):
        self.send_str += string + ";"

    def appendb(self, byte):
        self.send_str += byte

    def send(self, string=None):
        if not string:
            string = self.send_str + "\r\n"

        self.serial.write(string)

    def sendb(self, byte=None):
        if not byte:
            byte = self.send_str

        self.serial.write(byte)

    def readb(self, n=1):
        return self.serial.read(n)


class KerbalSimpit:
    KSP_PLUGIN_VERSION = b"1.1.5"
    MAX_PAYLOAD_SIZE = 32

    HDR = b'\xAAP'

    class ReceiveState:
        WaitingFirstByte = 0
        WaitingSecondByte = 1
        WaitingSize = 2
        WaitingType = 3
        WaitingData = 4

    def __init__(self, port):
        self.comm = Communication(port=port)
        self.recv_state = KerbalSimpit.ReceiveState.WaitingFirstByte

        self._messageHandler = KerbalSimpit.default_message_handler

        self.in_size = 0
        self.in_type = 0
        self.in_index = 0
        self.in_buffer = []

    def init(self):
        self.recv_state = KerbalSimpit.ReceiveState.WaitingFirstByte

        self.send(CommonPackets.SYNC_MESSAGE,
                  to_bytes(CommonPackets.SYNC_MESSAGE)
                    + KerbalSimpit.KSP_PLUGIN_VERSION)

        if self.comm.readb(1)[0] == KerbalSimpit.HDR[0]:
            if self.comm.readb(1)[0] == KerbalSimpit.HDR[1]:
                self.comm.readb(1)  # Drop size

                if self.comm.readb(1)[0] == CommonPackets.SYNC_MESSAGE:
                    if self.comm.readb(1)[0] == CommonPackets.ECHO_REQ_MESSAGE:
                        self.send(CommonPackets.SYNC_MESSAGE,
                                  to_bytes(CommonPackets.ECHO_RESP_MESSAGE)
                                    + KerbalSimpit.KSP_PLUGIN_VERSION)
                        print(" SIMPIT OK")
                        return True

        print(".", end="")
        return False

    @staticmethod
    def default_message_handler(messageType, msgbytes):
        print("RECV(TYPE={0}, data={1})".format(messageType, msgbytes))

    def inboundHandler(self, handler):
        self._messageHandler = handler

    def registerChannel(self, channelID):
        self.send(InboundPackets.REGISTER_MESSAGE, to_bytes(channelID))

    def deregisterChannel(self, channelID):
        self.send(InboundPackets.DEREGISTER_MESSAGE, to_bytes(channelID))

    def send(self, messageType, msgbytes):
        self.comm.resetb()
        self.comm.appendb(KerbalSimpit.HDR)
        self.comm.appendb(to_bytes(len(msgbytes)))
        self.comm.appendb(to_bytes(messageType))
        self.comm.appendb(msgbytes)
        self.comm.sendb()

    def update(self):
        for b in self.comm.readb():
            if self.recv_state == KerbalSimpit.ReceiveState.WaitingFirstByte:
                if b == KerbalSimpit.HDR[0]:
                    self.recv_state = KerbalSimpit.ReceiveState.WaitingSecondByte
                else:
                    self.recv_state = KerbalSimpit.ReceiveState.WaitingFirstByte

            elif self.recv_state == KerbalSimpit.ReceiveState.WaitingSecondByte:
                if b == KerbalSimpit.HDR[1]:
                    self.recv_state = KerbalSimpit.ReceiveState.WaitingSize
                else:
                    self.recv_state = KerbalSimpit.ReceiveState.WaitingFirstByte

            elif self.recv_state == KerbalSimpit.ReceiveState.WaitingSize:
                if b > KerbalSimpit.MAX_PAYLOAD_SIZE:
                    self.recv_state = KerbalSimpit.ReceiveState.WaitingFirstByte
                else:
                    self.in_size = int(b)
                    self.in_buffer = [0 for _ in range(self.in_size)]
                    self.recv_state = KerbalSimpit.ReceiveState.WaitingType

            elif self.recv_state == KerbalSimpit.ReceiveState.WaitingType:
                self.in_type = int(b)
                self.in_index = 0
                self.recv_state = KerbalSimpit.ReceiveState.WaitingData

            elif self.recv_state == KerbalSimpit.ReceiveState.WaitingData:
                self.in_buffer[self.in_index] = int(b)
                self.in_index += 1

                if self.in_index == self.in_size:
                    self.recv_state = KerbalSimpit.ReceiveState.WaitingFirstByte
                    if self._messageHandler:
                        self._messageHandler(self.in_type, self.in_buffer)

            else:
                self.recv_state = KerbalSimpit.ReceiveState.WaitingFirstByte

    def setSASMode(self, mode):
        self.send(InboundPackets.SAS_MODE_MESSAGE, mode)


###############################################################################
# Microbit communication
###############################################################################

class DeviceInput:
    STICK_X = "STICK_X"
    STICK_Y = "STICK_Y"

    BMODE_1 = "BMODE_1"     # STICK
    BMODE_2 = "BMODE_2"     # MOTION
    
    THROTTLE = "THROTTLE"
    
    TRIGGER_LEFT = "TRIGGER_LEFT"
    TRIGGER_RIGHT = "TRIGGER_RIGHT"

    ORIENT_FRONTBACK = "ORIENT_FRONTBACK"
    ORIENT_LEFTRIGHT = "ORIENT_LEFTRIGHT"
    ORIENT_TOPBOTTOM = "ORIENT_TOPBOTTOM"


class DeviceMessage:
    HDR = b'\xaa\x55'

    def __init__(self):
        self.states = {
            name: 0 for name in DeviceInput.__dict__ if not name.startswith("__")
        }
    
        self.size = len(self.bytes())
        
    def setState(self, name, value):
        self.states[name] = value
        
    def getState(self, name):
        return self.states[name]
    
    def bytes(self):
        return DeviceMessage.HDR \
             + to_bytes(self.getState(DeviceInput.STICK_X), 
                        self.getState(DeviceInput.STICK_Y),
                        self.getState(DeviceInput.ORIENT_FRONTBACK),
                        self.getState(DeviceInput.ORIENT_LEFTRIGHT),
                        self.getState(DeviceInput.ORIENT_TOPBOTTOM),
                        self.getState(DeviceInput.THROTTLE),
                        type=ct.c_int16) \
             + to_bytes((int(self.getState(DeviceInput.BMODE_1)) << 0) |
                        (int(self.getState(DeviceInput.BMODE_2)) << 1) |
                        (int(self.getState(DeviceInput.TRIGGER_LEFT)) << 2) |
                        (int(self.getState(DeviceInput.TRIGGER_RIGHT)) << 3)
                        )
    
    def fromBytes(self, byte):
        if len(byte) != self.size:
            return

        if byte[0:2] != DeviceMessage.HDR:
            return

        ints = list(to_ints_iterate(byte))[1:]
        
        self.states[DeviceInput.STICK_X]          = translate_stick_X(ints[0])
        self.states[DeviceInput.STICK_Y]          = translate_stick_Y(ints[1])
        self.states[DeviceInput.ORIENT_FRONTBACK] = translate_1000s_to_int16(ints[2])
        self.states[DeviceInput.ORIENT_LEFTRIGHT] = translate_1000s_to_int16(ints[3])
        self.states[DeviceInput.ORIENT_TOPBOTTOM] = translate_1000s_to_int16(ints[4])
        self.states[DeviceInput.THROTTLE]         = translate_1000u_to_int16(ints[5])
        self.states[DeviceInput.BMODE_1]          = (ints[6] & 0x1) != 0
        self.states[DeviceInput.BMODE_2]          = (ints[6] & 0x2) != 0
        self.states[DeviceInput.TRIGGER_LEFT]     = (ints[6] & 0x4) != 0
        self.states[DeviceInput.TRIGGER_RIGHT]    = (ints[6] & 0x8) != 0

    def fromString(self, string):
        if not string.endswith(b"\r\n"):
            return

        ints = list(map(int, string[:-2].split(b';')))

        if len(ints) != 10:
            return

        self.states[DeviceInput.STICK_X] = translate_stick_X(ints[0])
        self.states[DeviceInput.STICK_Y] = translate_stick_Y(ints[1])
        self.states[DeviceInput.ORIENT_FRONTBACK] = translate_1000s_to_int16(ints[2])
        self.states[DeviceInput.ORIENT_LEFTRIGHT] = translate_1000s_to_int16(ints[3])
        self.states[DeviceInput.ORIENT_TOPBOTTOM] = translate_1000s_to_int16(ints[4])
        self.states[DeviceInput.THROTTLE] = translate_1000u_to_int16(ints[5])
        self.states[DeviceInput.BMODE_1] = ints[6] != 0
        self.states[DeviceInput.BMODE_2] = ints[7] != 0
        self.states[DeviceInput.TRIGGER_LEFT] = ints[8] != 0
        self.states[DeviceInput.TRIGGER_RIGHT] = ints[9] != 0

        # print(";".join("{0:d}".format(x) for x in self.states.values()))

        self.states[DeviceInput.THROTTLE] = 0 if self.states[DeviceInput.THROTTLE] < 100 else self.states[DeviceInput.THROTTLE]


class MicroPlayer:
    def __init__(self, from_uart, device_serial_port, ksp_serial_port, baud=115200, recv_as_byte=False):
        self.from_uart = from_uart
        self.as_bytes  = recv_as_byte

        self.simpit = KerbalSimpit(port=ksp_serial_port)
        self.state = DeviceMessage()

        if self.from_uart:
            trying = True

            while trying:
                trying = False
                try:
                    self.conn = serial.Serial(port=device_serial_port, baudrate=baud)
                except Exception as e:
                    print("ERROR: " + str(e))
                    trying = input("Try again? (Y/N) ").lower() == 'y'
        else:
            # BLE
            self.conn = serial.Serial(port="/dev/rfcomm0")

    def get_data(self):
        if self.from_uart:
            if self.as_bytes:
                data = b''

                while data != DeviceMessage.HDR:
                    data = self.conn.read(2)

                data += self.conn.read(self.state.size - 2)
            else:
                data = self.conn.readline()
        else:
            # From BLE
            data = b''

        # print(data)

        if self.as_bytes:
            self.state.fromBytes(data)
        else:
            self.state.fromString(data)

        # print(self.state.states)

    def start(self):
        # Wait for both buttons
        print("Waiting for input, put KSP in foreground and press both triggers...")
        while True:
            self.get_data()

            if self.state.getState(DeviceInput.TRIGGER_LEFT) and self.state.getState(DeviceInput.TRIGGER_RIGHT):
                print("Waiting for sync...", end="")
                while not self.simpit.init():
                    pass
                break

        # Main loop
        rotmsg = RotationMessage()

        print("Start!")
        while True:
            # Get data from Microbit
            self.get_data()

            # Send data to KSP
            rotmsg.reset()

            if self.state.getState(DeviceInput.BMODE_2):
                # MOTION INPUT
                rotmsg.setPitch(self.state.getState(DeviceInput.ORIENT_FRONTBACK))
                rotmsg.setRoll(self.state.getState(DeviceInput.ORIENT_LEFTRIGHT))
            else:
                # STICK INPUT
                rotmsg.setPitch(self.state.getState(DeviceInput.STICK_Y))
                rotmsg.setRoll(-self.state.getState(DeviceInput.STICK_X))

            rotmsg.setYaw(-20000 if self.state.getState(DeviceInput.TRIGGER_LEFT) else \
                           20000 if self.state.getState(DeviceInput.TRIGGER_RIGHT) else 0)

            self.simpit.send(InboundPackets.ROTATION_MESSAGE,
                             rotmsg.bytes())

            self.simpit.send(InboundPackets.THROTTLE_MESSAGE,
                             to_bytes(self.state.getState(DeviceInput.THROTTLE), type=ct.c_int16))

            # Get data from KSP to display (not used)
            # self.simpit.update()


if __name__ == '__main__':
    g = MicroPlayer(from_uart=True, device_serial_port="COM7", ksp_serial_port="COM8")
    g.start()
