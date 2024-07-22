//Grid Initialization
//This script will save the entitys in dictionary to customdata when press running;
//Only works for CURRENT GRID, plz place the PB on which's initialization needed;
//Includes thruster, battery, o2tank, h2tank, block that has inventory, antenna, beacon, power-generator, gas-generator, gyro;
//Run this script with "DockOn", it'll set battery to "Recharge", thruster to "turn-off", beacon to "turn-off", gyro to "turn-off" and vice versa for argument "Dockoff";
//If the type is like "IMyPowerProducer" or "IMyShipController", plz try to edit "setBlock" func directly.

List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
MyIni _ini = new MyIni();
string errTxt = "";
IMyCubeGrid operationGrid;

public Program(){
    _ini.Clear(); _ini.TryParse(Me.CustomData); operationGrid = Me.CubeGrid;
    scriptDisplay("Grid Config");
    GridTerminalSystem.GetBlocksOfType(allBlocks);
    errTxt += $"Operation grid's name:\n{operationGrid.CustomName}\nRun the script to initialize.\n"; Echo(errTxt);
}

public void Main(string argument) {
    errTxt = "";
    if(argument == "") {
        _ini.Clear(); Me.CustomData = ""; _ini.TryParse(Me.CustomData);
        setBlocks(); Me.CustomData = _ini.ToString();
        errTxt += $"{operationGrid.CustomName}\n(ID: {operationGrid.EntityId})\ninitialize completed.";
    }
    else if (argument == "DockOn")
    {
        foreach (var key in getBlocks("Thruster")) { IMyThrust t = GridTerminalSystem.GetBlockWithId(key) as IMyThrust; t.Enabled = false; }
        foreach (var key in getBlocks("Battery")) { IMyBatteryBlock b = GridTerminalSystem.GetBlockWithId(key) as IMyBatteryBlock; b.ChargeMode = ChargeMode.Recharge; }
        foreach (var key in getBlocks("Beacon")) { IMyBeacon b = GridTerminalSystem.GetBlockWithId(key) as IMyBeacon; b.Enabled = false; }
        foreach (var key in getBlocks("Gyro")) { IMyGyro g = GridTerminalSystem.GetBlockWithId(key) as IMyGyro; g.Enabled = false; }
        errTxt += "Dock-on setting applied.";
    }
    else if (argument == "DockOff") {
        foreach (var key in getBlocks("Thruster")) { IMyThrust t = GridTerminalSystem.GetBlockWithId(key) as IMyThrust; t.Enabled = true; }
        foreach (var key in getBlocks("Battery")) { IMyBatteryBlock b = GridTerminalSystem.GetBlockWithId(key) as IMyBatteryBlock; b.ChargeMode = ChargeMode.Auto; }
        foreach (var key in getBlocks("Beacon")) { IMyBeacon b = GridTerminalSystem.GetBlockWithId(key) as IMyBeacon; b.Enabled = true; }
        foreach (var key in getBlocks("Gyro")) { IMyGyro g = GridTerminalSystem.GetBlockWithId(key) as IMyGyro; g.Enabled = true; }
        errTxt += "Dock-off setting applied.";
    }
    Echo(errTxt);
}

void setBlocks() {
    foreach (Type key in trans.Keys) {
        string section = trans[key];
        foreach (var block in allBlocks) {
            if(block.CubeGrid.EntityId == operationGrid.EntityId)
            {
                block.ShowInToolbarConfig = false; block.ShowInTerminal = false;
                string keyName = "I" + block.GetType().Name;
                if (keyName == key.Name){
                    if (key == typeof(IMyGasTank)){
                        if (block.BlockDefinition.SubtypeId.Contains("Hydrogen")) { _ini.Set("H2Tank", illegalChk(block.CustomName), block.EntityId.ToString()); }
                        else { _ini.Set("O2Tank", illegalChk(block.CustomName), block.EntityId.ToString()); }
                    }
                    else { _ini.Set(section, illegalChk(block.CustomName), block.EntityId.ToString()); }
                }
                if (block.HasInventory) { _ini.Set("Inventory", illegalChk(block.CustomName), block.EntityId.ToString()); }
                if (block is IMyPowerProducer && !(block is IMyBatteryBlock)) { _ini.Set("Generator", illegalChk(block.CustomName), block.EntityId.ToString()); }
            }
        }
    }
}

string illegalChk(string str) { return str.Replace("|", "").Replace(",", "").Replace("=", ""); }

List<long> getBlocks(string section)
{
    List<long> keyls = new List<long>();
    _ini.TryParse(Me.CustomData);
    List<MyIniKey> Keys = new List<MyIniKey>(); _ini.GetKeys(section, Keys);
    if (!_ini.ContainsSection(section)) { return keyls; }
    foreach (var key in Keys)
    {
        keyls.Add(_ini.Get(key).ToInt64());
    }
    return keyls;
}

long NameToId(string section, string key)
{
    long entityId;
    string entityName;
    if (!_ini.Get(section, key).TryGetInt64(out entityId))
    {
        if (!_ini.Get(section, key).TryGetString(out entityName))
        {
            _ini.Set(section, key, "[Enter its name.]");
            Me.CustomData = _ini.ToString();
            errTxt += $"Error: cant get {key}'s value,\nPlz re-enter its name then reset the script.";
            Echo(errTxt + "\n");
            return 0;
        }
        IMyTerminalBlock entity = GridTerminalSystem.GetBlockWithName(_ini.Get(section, key).ToString());
        if (entity == null)
        {
            errTxt += $"Error: the {key} called {_ini.Get(section, key).ToString()} is not found.";
            Echo(errTxt + "\n");
            return 0;
        }
        _ini.Set(section, key, entity.EntityId.ToString());
        Me.CustomData = _ini.ToString();
    }
    return entityId;
}

void scriptDisplay(string scriptTitle)
{
    Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(1).FontSize = 4.1f;
    Me.GetSurface(1).FontColor = new Color(255, 255, 0);
    Me.GetSurface(1).Alignment = TextAlignment.CENTER;
    Me.GetSurface(1).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(1).WriteText("\n" + scriptTitle);
}

Dictionary<Type, string> trans = new Dictionary<Type, string>() {
    { typeof(IMyBatteryBlock), "Battery"},
    { typeof(IMyThrust), "Thruster"},
    { typeof(IMyRadioAntenna), "Antenna"},
    { typeof(IMyBeacon), "Beacon"},
    { typeof(IMyGyro), "Gyro"},
    { typeof(IMyGasTank), "GasTank"},
};