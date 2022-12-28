using UnityEngine;
using System.IO.Ports;

public class SerialHandler : MonoBehaviour
{
    private SerialPort _serial;

    [SerializeField] private string serialPort = "COM3";
    [SerializeField] private int baudrate = 115200;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Serial connection configuration and opening the connection
        _serial = new SerialPort(serialPort, baudrate);
        _serial.NewLine = "\n";
        _serial.Open();
    }

    private void Update()
    {
        if (_serial.BytesToRead <= 0) return;

        var message = _serial.ReadLine().Trim();

        switch (message)
        {
            case "tmr pp":
                Timer.instance.PauseSwitchTimer();
                break;
            case "tmr rt":
                Timer.instance.ResetTimer();
                break;
            case "tmr sc":
                Timer.instance.SwitchTimerShowState();
                _serial.WriteLine("time " + Timer.instance.currentTime);
                break;
            default:
                Debug.Log("Unknown serial message.");
                break;
        }
    }
}