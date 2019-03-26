from pymouse import PyMouse
from pykeyboard import PyKeyboard
import serial

## https://github.com/PyUserInput/PyUserInput

#m = PyMouse()
#x_dim, y_dim = m.screen_size()

k = PyKeyboard()

class KeyboardInput:
    KEY_FORWARD = 'W'
    KEY_BACK    = 'S'
    KEY_LEFT    = 'A'
    KEY_RIGHT   = 'D'

    KEY_Q = 'Q'
    KEY_Z = 'Z'
    KEY_E = 'E'
    KEY_F = 'F'

    KEY_ARROW_UP    = 'UP'
    KEY_ARROW_DOWN  = 'DOWN'
    KEY_ARROW_LEFT  = 'LEFT'
    KEY_ARROW_RIGHT = 'RIGHT'

    KEYS = {
        # Qwerty
        KEY_FORWARD: 0x57,
        KEY_BACK   : 0x53,
        KEY_LEFT   : 0x41,
        KEY_RIGHT  : 0x44,
        KEY_Q      : 0x51,
        KEY_Z      : 0x5A,

        # # Azerty
        # KEY_FORWARD: 0x5A,
        # KEY_BACK   : 0x53,
        # KEY_LEFT   : 0x51,
        # KEY_RIGHT  : 0x44,
        # KEY_Q      : 0x41,
        # KEY_Z      : 0x57,

        KEY_E: 0x45,
        KEY_F: 0x46,

        KEY_ARROW_UP   : k.up_key,
        KEY_ARROW_DOWN : k.down_key,
        KEY_ARROW_LEFT : k.left_key,
        KEY_ARROW_RIGHT: k.right_key,
    }

    def __init__(self):
        self.keys = {
            key: False for key in KeyboardInput.KEYS.keys()
        }

        self.keyboard = k

    def __del__(self):
        for key, state in self.keys.items():
            self.keyboard.release_key(KeyboardInput.KEYS[key])

    def changeKeyState(self, key, value):
        # Change only necessary

        value = bool(value)

        if value != self.keys[key]:
            print("Key {0} => {1}".format(key, value))
            code = KeyboardInput.KEYS[key]
            if value:
                self.keyboard.press_key(code)
            else:
                self.keyboard.release_key(code)

        self.keys[key] = value


class MicroPlayer:
    def __init__(self, from_uart=True, port="COM3", baud=115200):
        self.from_uart = from_uart
        self.keyboard  = KeyboardInput()

        self.states = self.keyboard.keys.copy()

        if self.from_uart:
            self.conn = serial.Serial(port=port, baudrate=baud)
        else:
            # BLE
            self.conn = None

    def get_data(self):
        data = b"0;0;(0, 0, 0);\r\n"

        if self.from_uart:
            # data = "0;1;(16, -48, -1008);\r\n"
            data = self.conn.readline()
        else:
            # From BLE
            data = data

        print(data)

        # Split inputs
        try:
            BTN_A, BTN_B, accel, _ = data.split(b';')
        except Exception as e:
            print("ERROR: '{0}':".format(str(data)) + str(e))
        else:
            x, y, z = list(map(float, accel[1:-1].split(b',')))

            self.states[KeyboardInput.KEY_E] = BTN_A == b'1'
            self.states[KeyboardInput.KEY_F] = BTN_B == b'1'

            self.states[KeyboardInput.KEY_FORWARD] = y < -400
            self.states[KeyboardInput.KEY_BACK]    = y > 400
            self.states[KeyboardInput.KEY_LEFT]    = x < -200
            self.states[KeyboardInput.KEY_RIGHT]   = x > 200

    def start(self):
        # Wait for both buttons
        print("Waiting for input, put game in foreground and press both buttons...")
        while True:
            self.get_data()

            if self.states[KeyboardInput.KEY_E] and self.states[KeyboardInput.KEY_F]:
                break

        # Main loop
        print("Start!")
        while True:
            self.get_data()

            # KSP
            self.keyboard.changeKeyState(KeyboardInput.KEY_Q, self.states[KeyboardInput.KEY_LEFT])
            self.keyboard.changeKeyState(KeyboardInput.KEY_E, self.states[KeyboardInput.KEY_RIGHT])

            self.keyboard.changeKeyState(KeyboardInput.KEY_FORWARD, self.states[KeyboardInput.KEY_FORWARD])
            self.keyboard.changeKeyState(KeyboardInput.KEY_BACK   , self.states[KeyboardInput.KEY_BACK])
            self.keyboard.changeKeyState(KeyboardInput.KEY_LEFT   , self.states[KeyboardInput.KEY_E])
            self.keyboard.changeKeyState(KeyboardInput.KEY_RIGHT  , self.states[KeyboardInput.KEY_F])

            # for key, state in self.states.items():
            #     self.keyboard.changeKeyState(key, state)


if __name__ == '__main__':
    g = MicroPlayer(from_uart=True)
    g.start()
