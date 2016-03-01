using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Layton.AuditWizard.DataAccess
{
    #region Printer Class

    public class Printer
    {
        #region Data

        public enum PrinterStatus
        {
            Other = 1,
            Unknown = 2,
            Idle = 3,
            Printing = 4,
            Warmup = 5
        }

        public enum PrinterType
        {
            Laser = 4,
            InkJetAqueous = 12,
            InkJetSolid = 13,
            InkJetOther = 14,
            DotMatrix9PIN = 6,
            DotMatrix24PIN = 7,
            DotMatrixOther = 8
        }

        // Printer Info
        private string _manufacturer;
        private string _modelName;
        private string _adminName;
        private int _memory;
        private string _ipAddress;
        private string _macAddress;
        private string _serialNumber;
        private DateTime _dateAudited;
        private DateTime _sysUpTime;
        private PrinterStatus _printerStatus;
        private PrinterType _printerType;

        // Printer Counter
        private int _pagesPrintedTotal;
        private int _pagesPrintedSinceReboot;

        // Printer Levels
        private List<PrinterLevel> _printerLevels = new List<PrinterLevel>();
        private List<PrinterInputTray> _printerInputTrays = new List<PrinterInputTray>();
        private List<PrinterOutputTray> _printerOutputTrays = new List<PrinterOutputTray>();

        #endregion

        #region Properties

        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }

        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; }
        }

        public string AdminName
        {
            get { return _adminName; }
            set { _adminName = value; }
        }

        public int Memory
        {
            get { return _memory; }
            set { _memory = value; }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        public string MacAddress
        {
            get { return _macAddress; }
            set { _macAddress = value; }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        public DateTime DateAudited
        {
            get { return _dateAudited; }
            set { _dateAudited = value; }
        }

        public DateTime SysUpTime
        {
            get { return _sysUpTime; }
            set { _sysUpTime = value; }
        }

        public PrinterStatus PrinterCurrentStatus
        {
            get { return _printerStatus; }
            set { _printerStatus = value; }
        }

        public int PagesPrintedTotal
        {
            get { return _pagesPrintedTotal; }
            set { _pagesPrintedTotal = value; }
        }

        public int PagesPrintedSinceReboot
        {
            get { return _pagesPrintedSinceReboot; }
            set { _pagesPrintedSinceReboot = value; }
        }

        public List<PrinterLevel> PrinterLevels
        {
            get { return _printerLevels; }
            set { _printerLevels = value; }
        }

        public List<PrinterInputTray> PrinterInputTrays
        {
            get { return _printerInputTrays; }
            set { _printerInputTrays = value; }
        }

        public List<PrinterOutputTray> PrinterOutputTrays
        {
            get { return _printerOutputTrays; }
            set { _printerOutputTrays = value; }
        }

        public PrinterType TypeOfPrinter
        {
            get { return _printerType; }
            set { _printerType = value; }
        }

        #endregion
    }

    #endregion

    #region PrinterLevel Class

    public class PrinterLevel
    {
        private string _mediaName;
        private int _mediaLevel;
        private SupplyType _mediaType;

        public enum SupplyType
        {
            Other = 1,
            Unknown,
            Toner,
            WasteToner,
            Ink,
            InkCartridge,
            InkRibbon,
            WasteInk,
            OPC,
            Developer,
            FuserOil,
            SolidWax,
            RibbonWax,
            WastWax,
            Fuser,
            CoronaWire,
            FuserOilWick,
            CleanerUnit,
            FuserCleaningPad,
            TransferUnit,
            TonerCartridge,
            FuserOiler,
            Water,
            WasteWater,
            GlueWaterAdditive,
            WasterPaper,
            BindingSupply,
            BandingSupply,
            StichingPaper,
            ShrinkPaper,
            PaperWrap,
            Staples,
            Inserts
        }

        public string MediaName
        {
            get { return _mediaName; }
            set { _mediaName = value; }
        }

        public int MediaLevel
        {
            get { return _mediaLevel; }
            set { _mediaLevel = value; }
        }

        public SupplyType MediaType
        {
            get { return _mediaType; }
            set { _mediaType = value; }
        }
    }

    #endregion

    #region PrinterInputTray Class

    public class PrinterInputTray
    {
        public enum PrinterTrayStatus
        {
            Empty = 0,
            Unknown = -2,
            OK = -3
        }

        private string _trayName;
        private int _trayCapacity;
        private string _paperType;
        private PrinterTrayStatus _trayStatus;

        public string TrayName
        {
            get { return _trayName; }
            set { _trayName = value; }
        }

        public int TrayCapacity
        {
            get { return _trayCapacity; }
            set { _trayCapacity = value; }
        }

        public string PaperType
        {
            get { return _paperType; }
            set { _paperType = value; }
        }

        public PrinterTrayStatus TrayStatus
        {
            get { return _trayStatus; }
            set { _trayStatus = value; }
        }
    }

    #endregion

    #region PrinterOutputTray Class

    public class PrinterOutputTray
    {
        public enum OutputStackingOrder
        {
            Unknown = 2,
            FirstToLast = 3,
            LastToFirst = 4
        }

        public enum OutputPageDelivery
        {
            FaceUp = 3,
            FaceDown = 4
        }

        private string _trayName;
        private int _trayCapacity;
        private OutputStackingOrder _stackingOrder;
        private OutputPageDelivery _pageOrientation;

        public string TrayName
        {
            get { return _trayName; }
            set { _trayName = value; }
        }

        public int TrayCapacity
        {
            get { return _trayCapacity; }
            set { _trayCapacity = value; }
        }

        public OutputStackingOrder StackingOrder
        {
            get { return _stackingOrder; }
            set { _stackingOrder = value; }
        }

        public OutputPageDelivery PageOrientation
        {
            get { return _pageOrientation; }
            set { _pageOrientation = value; }
        }
    }

    #endregion
}
