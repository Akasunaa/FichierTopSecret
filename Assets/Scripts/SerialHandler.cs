using System;
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
            case "timer p":
                Timer.instance.PauseSwitchTimer();
                break;
            case "timer r":
                Timer.instance.ResetTimer();
                break;
            case "timer s":
                Timer.instance.SwitchTimerShowState();
                break;
        }
    }
}