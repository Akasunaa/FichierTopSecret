using System.Collections;
using System.Globalization;
using UnityEngine;
using System.IO.Ports;
using System.Linq;

public class SerialHandler : MonoBehaviour
{
    private SerialPort _serial;

    [SerializeField] private string serialPort = "COM3";
    [SerializeField] private int baudRate = 115200;

    private bool _isTimerHandledHere = true;

    private bool[] _syncParams;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Serial connection configuration and opening the connection
        _serial = new SerialPort(serialPort, baudRate);
        _serial.NewLine = "\n";
        _serial.Open();
        _syncParams = new[] { _isTimerHandledHere, Timer.instance.isRunning };
    }

    private void Update()
    {
        if (_serial.BytesToRead <= 0) return;

        var message = _serial.ReadLine().Trim();

        var messageArgs = message.Split(' ');

        if (messageArgs[0] == null || messageArgs[1] == null) return;
        
        switch (messageArgs[0])
        {
            case "tmr":
                switch (messageArgs[1])
                {
                    case "pp":
                        Timer.instance.PauseSwitchTimer();
                        break;
                    case "rt":
                        Timer.instance.ResetTimer();
                        break;
                    case "sh":
                        if(_isTimerHandledHere) 
                            Timer.instance.SwitchTimerShowState();
                        break;
                    case "sw":
                        if (_isTimerHandledHere)
                        {
                            StartCoroutine(SwitchToArduino());
                        }
                        else
                            _isTimerHandledHere = true;
                        
                        if(!Timer.instance.isShown) Timer.instance.SwitchTimerShowState();
                        break;
                    default:
                        Debug.Log("Unknown serial instruction.");
                        break;
                }
                break;
            case"stg":
                switch (messageArgs[1])
                {
                    case "sy":
                        _serial.WriteLine("sync " + SyncParamsToString());
                        break;
                    case "rb":
                        Debug.Log("Arduino rebooting...");
                        break;
                    case "rd":
                        Debug.Log("Arduino ready.");
                        break;
                    default:
                        Debug.Log("Unknown serial instruction.");
                        break;
                }
                break;
            case "sync":
            {
                for (var i = 0; i < messageArgs[1].Length; i++)
                {
                    _syncParams[i] = messageArgs[1][i] == '1';
                }

                _syncParams[0] = !_syncParams[0]; // cuz arduino sent that IT was in charge of that timer
                
                _isTimerHandledHere = _syncParams[0];
                Timer.instance.isRunning = _syncParams[1];
                
                break;
            }
            case "time":
                Timer.instance.currentTime = float.Parse(messageArgs[1], CultureInfo.InvariantCulture);
                break;
            default:
                Debug.Log("Unknown serial instruction.");
                break;
        }
    }

    private IEnumerator SwitchToArduino()
    {
        _serial.WriteLine("sync " + SyncParamsToString());
        yield return new WaitForSecondsRealtime(0.5f);
        _serial.WriteLine("time " + Timer.instance.currentTime);
        
        if(Timer.instance.isShown) Timer.instance.SwitchTimerShowState();
        _isTimerHandledHere = false;
    }
    
    private string SyncParamsToString()
    {
        _syncParams = new[] { _isTimerHandledHere, Timer.instance.isRunning };
        return _syncParams.Aggregate("", (current, b) => current + (b ? "1" : "0"));
    }
}