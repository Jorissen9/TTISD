# Write your code here :-)
from microbit import *
import radio

display.clear()

class Communication:
    def __init__(self, use_uart=True):
        self.use_uart = use_uart
        self.send_str = ""

        if self.use_uart:
            uart.init(baudrate=115200)
        else:
            # Turn the radio on. (BLE)
            radio.config(length=32, queue=3, channel=7, power=0)
            radio.on()

    def reset(self):
        self.send_str = ""

    def append(self, string):
        self.send_str += string + ";"

    def send(self, string=None):
        if not string:
            string = self.send_str + "\r\n"

        if self.use_uart:
            uart.write(string)
        else:
            radio.send(string)

    def sendb(self, byte):
        if self.use_uart:
            uart.write(byte)
        else:
            radio.send_bytes(byte)


comm = Communication(use_uart=True)
comm.send("Start...")
display.show(Image.HAPPY)

while True:
    comm.reset()

    # Send BTN_A
    comm.append(str(int(button_a.is_pressed())))

    # Send BTN_B
    comm.append(str(int(button_b.is_pressed())))

    # Send ACCEL
    ac_x, ac_y, ac_z = accelerometer.get_values()
    comm.append("({0:d},{1:d},{2:d})".format(ac_x, ac_y, ac_z))

    comm.send()
    sleep(50)

comm.send("Quit...")
display.show(Image.SAD)
sleep(2000)
display.clear()