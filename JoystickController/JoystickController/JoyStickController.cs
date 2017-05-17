using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VRage.Plugins;
using VRage.Input;
using VRage.Utils;
using Sandbox.Engine.Utils;
using Sandbox.Game;

namespace JoystickController {
    public class JoyStickController : IPlugin {
        IEnumerable<MyJoystickAxesEnum> axes = Enum.GetValues(typeof(MyJoystickAxesEnum)).Cast<MyJoystickAxesEnum>();
        IEnumerable<MyJoystickButtonsEnum> buttons = Enum.GetValues(typeof(MyJoystickButtonsEnum)).Cast<MyJoystickButtonsEnum>();
        bool logging = false;
        public void Dispose() {
            //do Nothing 
        }
        public void Init(object gameInstance) {
            try {
                Dictionary<string, string> configPairs = loadConfigFile();
                setUpControls(configPairs);
            } catch( Exception e ) {
                throw new Exception("Error occured during loading of configuration please double check the config is valid ");
            }
        }


        private Dictionary<string, string> loadConfigFile() {
            Dictionary<string, string> configPairs = new Dictionary<string, string>();
            MyLog.Default.WriteLine("Attempting to load " + Environment.CurrentDirectory + "\\JoyStickController.cfg");
            foreach( string line in File.ReadLines(Environment.CurrentDirectory + "\\JoyStickController.cfg") ) {
                if( !(line.StartsWith("#")) ) {
                    if( line.StartsWith("LOGGING") ) {
                        logging = Boolean.Parse(line.Split('=')[1].Trim());
                    } else {
                        string[] splitLine = line.Split('=');
                        string controlAction = splitLine[0].Trim();
                        string controlBind = splitLine[1].Trim();
                        configPairs.Add(controlAction, controlBind);
                    }
                }
            }
            return configPairs;
        }
        private void setUpControls(Dictionary<string, string> configPairs) {
            foreach( KeyValuePair<string, string> entry in configPairs ) {
                string value = "";
                if( entry.Value.StartsWith("BUTTON.") ) {
                    MyJoystickButtonsEnum enumedValue;
                    value = entry.Value.Replace("BUTTON.", "");
                    Enum.TryParse(value, out enumedValue);
                    MyControllerHelper.AddControl(MySpaceBindingCreator.CX_CHARACTER, MyStringId.GetOrCompute(entry.Key), enumedValue);
                    MyLog.Default.WriteLine("Setting " + entry.Key + " To " + value);
                }
                if( entry.Value.StartsWith("JOYSTICK.") ) {
                    MyJoystickAxesEnum enumedValue;
                    value = entry.Value.Replace("JOYSTICK.", "");
                    Enum.TryParse(value, out enumedValue);
                    MyControllerHelper.AddControl(MySpaceBindingCreator.CX_CHARACTER, MyStringId.GetOrCompute(entry.Key), enumedValue);
                    MyLog.Default.WriteLine("Setting " + entry.Key + " To " + value);
                }
            }
        }
        public void Update() {
            if( logging ) {
                foreach( MyJoystickAxesEnum axis in axes ) {
                    if( !(axis.Equals(MyJoystickAxesEnum.None)) ) {
                        if( MyInput.Static.IsJoystickAxisNewPressed(axis) || MyInput.Static.IsJoystickAxisPressed(axis) ) {
                            MyLog.Default.WriteLine("Pressed " + axis);
                        }
                    }
                }
                foreach( MyJoystickButtonsEnum button in buttons ) {
                    if( !(button.Equals(MyJoystickButtonsEnum.None)) ) {
                        if( MyInput.Static.IsJoystickButtonNewPressed(button) || MyInput.Static.IsJoystickButtonPressed(button) ) {
                            MyLog.Default.WriteLine("Pressed " + button);
                        }
                    }
                }
            }
        }

    }
}
