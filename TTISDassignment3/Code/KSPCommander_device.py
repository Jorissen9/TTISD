from microbit import *
#import radio  # Not enough memory to use with micropython...

class Type:
    def __init__(self, m, b, s):
        self.m = m
        self.b = b
        self.s = s

class ct:
    c_uint8 = Type(  0xFF, 1, False)
    c_int16 = Type(0xFFFF, 2, True )


def to_bytes(*args, typemask=ct.c_uint8):
    return b''.join((x & typemask.m) \
                    .to_bytes(typemask.b,
                              'big',
                              typemask.s)
                for x in args)


class DeviceInput:
    STICK_X = "STICK_X"
    STICK_Y = "STICK_Y"

    BMODE_1 = "BMODE_1"
    BMODE_2 = "BMODE_2"

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
            # Gives error in micropython...
            # name: 0 for name in DeviceInput.__dict__ if not name.startswith("__")

            DeviceInput.STICK_X: 0,
            DeviceInput.STICK_Y: 0,

            DeviceInput.BMODE_1: False,
            DeviceInput.BMODE_2: False,

            DeviceInput.THROTTLE: 0,
            DeviceInput.TRIGGER_LEFT: False,
            DeviceInput.TRIGGER_RIGHT: False,

            DeviceInput.ORIENT_FRONTBACK: 0,
            DeviceInput.ORIENT_LEFTRIGHT: 0,
            DeviceInput.ORIENT_TOPBOTTOM: 0,
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
                        typemask=ct.c_int16) \
             + to_bytes((int(self.getState(DeviceInput.BMODE_1)) << 0) |
                        (int(self.getState(DeviceInput.BMODE_2)) << 1) |
                        (int(self.getState(DeviceInput.TRIGGER_LEFT)) << 2) |
                        (int(self.getState(DeviceInput.TRIGGER_RIGHT)) << 3)
                        )

    def string(self):
        return str(self.getState(DeviceInput.STICK_X)           ) + ";" + \
               str(self.getState(DeviceInput.STICK_Y)           ) + ";" + \
               str(self.getState(DeviceInput.ORIENT_FRONTBACK)  ) + ";" + \
               str(self.getState(DeviceInput.ORIENT_LEFTRIGHT)  ) + ";" + \
               str(self.getState(DeviceInput.ORIENT_TOPBOTTOM)  ) + ";" + \
               str(self.getState(DeviceInput.THROTTLE)          ) + ";" + \
               str(int(self.getState(DeviceInput.BMODE_1))      ) + ";" + \
               str(int(self.getState(DeviceInput.BMODE_2))      ) + ";" + \
               str(int(self.getState(DeviceInput.TRIGGER_LEFT)) ) + ";" + \
               str(int(self.getState(DeviceInput.TRIGGER_RIGHT)))

    def __str__(self):
        return ", ".join("{0:s}: {1:d}".format(k, v) for k, v in self.states.items())


class Communication:
    def __init__(self, use_uart=True):
        self.use_uart = use_uart
        self.send_str = ""

        if self.use_uart:
            uart.init(baudrate=115200)
        else:
            # Turn the radio on. (BLE)
            #radio.config(length=32, queue=3, channel=7, power=0)
            #radio.on()
            pass

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

        if self.use_uart:
            uart.write(string)
        else:
            #radio.send(string)
            pass

    def sendb(self, byte=None):
        if not byte:
            byte = self.send_str

        if self.use_uart:
            uart.write(byte)
        else:
            #radio.send_bytes(byte)
            pass


if __name__ == '__main__':
    display.clear()

    comm = Communication(use_uart=True)
    comm.send("Start...")
    display.show(Image.HAPPY)

    msg = DeviceMessage()

    while True:
        # Send stick (X,Y)
        msg.setState(DeviceInput.STICK_X, pin0.read_analog())
        msg.setState(DeviceInput.STICK_Y, pin1.read_analog())

        # Send BTN_A (trigger left)
        msg.setState(DeviceInput.TRIGGER_LEFT, button_a.is_pressed())

        # Send BTN_B (trigger right)
        msg.setState(DeviceInput.TRIGGER_RIGHT, button_b.is_pressed())

        # Send Mode
        mode = pin16.read_digital()
        msg.setState(DeviceInput.BMODE_1, mode)
        msg.setState(DeviceInput.BMODE_2, not mode)

        # Send Throttle
        msg.setState(DeviceInput.THROTTLE, pin2.read_analog())

        # Send ACCEL (X,Y,Z)
        ac_x, ac_y, ac_z = accelerometer.get_values()
        msg.setState(DeviceInput.ORIENT_FRONTBACK, ac_y)
        msg.setState(DeviceInput.ORIENT_LEFTRIGHT, ac_x)
        msg.setState(DeviceInput.ORIENT_TOPBOTTOM, ac_z)

        #comm.resetb()
        #comm.appendb(msg.bytes())
        #comm.sendb()

        comm.reset()
        comm.send_str = msg.string()
        comm.send()

        sleep(50)

    comm.send("Quit...")
    display.show(Image.SAD)
    sleep(2000)
    display.clear()