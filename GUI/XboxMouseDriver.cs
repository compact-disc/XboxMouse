using System;
using System.Threading;
using SharpDX.XInput;

namespace XboxMouse
{
    internal class XboxMouseDriver
    {
        //Mouse event method to be called with parameters for mouse functions
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        //Keyboard event function import
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        //Mouse left click values
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        //Mouse middle click values
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;

        //Mouse right click values
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        //Mouse move value
        private const int MOUSEEVENTF_MOVE = 0x0001;

        //Mouse wheel movement
        private const int MOUSEEVENTF_WHEEL = 0x0800;

        //Mouse tilt wheel movement
        private const int MOUSEEVENTF_HWHEEL = 0x01000;

        //Browser back key
        private const byte VK_BROWSER_BACK = 0xA6;

        //Keyboard press key action
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;

        //Keyboard release key action
        private const int KEYEVENTF_KEYUP = 0x0002;

        public XboxMouseDriver()
        {

            //Variables for mouse movement and acceleration
            double mXA0 = -32678, mXA1 = 32767, mXB0 = -15, mXB1 = 15, mXA;
            double mYA0 = -32678, mYA1 = 32767, mYB0 = 15, mYB1 = -15, mYA;
            double mXMovement, mYMovement;

            //Variables for scroll movement and acceleration
            double sXA0 = -32678, sXA1 = 32767, sXB0 = -10, sXB1 = 10, sXA;
            double sYA0 = -32678, sYA1 = 32767, sYB0 = -25, sYB1 = 25, sYA;
            double sXMovement, sYMovement;

            //Opening message with instructions how to exit
            Console.WriteLine("Xbox Mouse Started. Press the ESCAPE key to exit.");

            //Create the controller object to be used, there is only a setup for one controller because there is only one mouse on a computer
            Controller controller = new Controller(UserIndex.One);

            //Check if the controller is connected and show message
            //Else show a message saying not connected
            if (controller.IsConnected)
            {
                Console.WriteLine("Controller Connected.");
            }
            else
            {
                Console.WriteLine("No Controller Connected. Waiting to connect or press ESCAPE to close.");

                while (true)
                {
                    //If the escape key is pressed then break the while loop to close the program
                    if (EscapeKeyPressed(ConsoleKey.Escape))
                    {
                        break;
                    }

                    if (controller.IsConnected)
                    {
                        Console.WriteLine("Controller Connected.");
                        break;
                    }

                    Thread.Sleep(10);
                }

            }

            //Only start the while loop to run the program if there is a controller connected
            while (controller.IsConnected)
            {

                //Get the state of the controller
                var state = controller.GetState();

                //Swich over the buttons to see which one is pressed
                switch (state.Gamepad.Buttons)
                {

                    //When the a button is pressed then left click
                    case GamepadButtonFlags.A:
                        LeftClick();
                        break;

                    //When the y button is pressed then middle click
                    case GamepadButtonFlags.Y:
                        MiddleClick();
                        break;

                    //When the b button is pressed then right click
                    case GamepadButtonFlags.B:
                        RightClick();
                        break;

                    //When the x button is pressed then alt+leftarrow
                    case GamepadButtonFlags.X:
                        BrowserBack();
                        break;

                }

                //Get the value for the left thumb stick but with a margin of error of 5000
                //Else do nothing at zero
                if (state.Gamepad.LeftThumbX >= 5000 || state.Gamepad.LeftThumbX <= -5000)
                {
                    mXA = state.Gamepad.LeftThumbX;
                }
                else
                {
                    mXA = 0;
                }

                //Get the value for the left thumb stick but with a margin of error of 5000
                //Else do nothing at zero
                if (state.Gamepad.LeftThumbY >= 5000 || state.Gamepad.LeftThumbY <= -5000)
                {
                    mYA = state.Gamepad.LeftThumbY;
                }
                else
                {
                    mYA = 0;
                }

                //Get the acceleration for the x and y values for mouse movement
                mXMovement = Acceleration(mXA0, mXA1, mXB0, mXB1, mXA);
                mYMovement = Acceleration(mYA0, mYA1, mYB0, mYB1, mYA);

                //Move the mouse with the given x and y change values
                MoveMouse((int)mXMovement, (int)mYMovement);

                //Get the values for the X stick but only if it is above 5000 for margin of error in the stick
                //Else set it to zero so it doesnt move by itself
                if (state.Gamepad.RightThumbX >= 5000 || state.Gamepad.RightThumbX <= -5000)
                {
                    sXA = state.Gamepad.RightThumbX;
                }
                else
                {
                    sXA = 0;
                }

                //Get the values for the Y stick but only if it is above 5000 for margin of error in the stick
                //Else set it to zero so it doesnt move by itself
                if (state.Gamepad.RightThumbY >= 5000 || state.Gamepad.RightThumbY <= -5000)
                {
                    sYA = state.Gamepad.RightThumbY;
                }
                else
                {
                    sYA = 0;
                }

                //Get the acceleration for the x and y values for scroll movement
                sXMovement = Acceleration(sXA0, sXA1, sXB0, sXB1, sXA);
                sYMovement = Acceleration(sYA0, sYA1, sYB0, sYB1, sYA);

                //Scroll the page given the x and y change values
                Scroll((int)sYMovement, (int)sXMovement);

                //Wait within the loop so it can actually read and work
                Thread.Sleep(10);

            }

            //When the program is ended, say it
            Console.WriteLine("Xbox Mouse Stopped.");

        }

        //Check for the escape key being pressed and return a boolean value
        private static bool EscapeKeyPressed(ConsoleKey key)
        {
            return Console.KeyAvailable && Console.ReadKey(true).Key == key;
        }

        //Left click the mouse
        private static void LeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(150);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        //Middle click the mouse
        private static void MiddleClick()
        {
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
            Thread.Sleep(150);
            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }

        //Right click the mouse
        private static void RightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            Thread.Sleep(150);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        //Scroll the page based on the horizontal and vertical scroll values changed
        private static void Scroll(int verticalMovement, int horizontalMovement)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, verticalMovement, 0);
            mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, horizontalMovement, 0);
        }

        //Move the mouse based on the change in x and y values
        private static void MoveMouse(int dx, int dy)
        {
            mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, 0);
        }

        //Acceleration calculation given the ranges of the controller
        //Correlate those ranges with the controller and return some value between movement min and max
        private static double Acceleration(double a0, double a1, double b0, double b1, double a)
        {
            return b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
        }

        //Run the keyboard shortcut for going back within a web browser
        private static void BrowserBack()
        {
            keybd_event(VK_BROWSER_BACK, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(150);
            keybd_event(VK_BROWSER_BACK, 0, KEYEVENTF_KEYUP, 0);
        }

    }
}