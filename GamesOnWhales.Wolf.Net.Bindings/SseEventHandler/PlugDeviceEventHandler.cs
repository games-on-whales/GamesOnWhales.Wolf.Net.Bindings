namespace GamesOnWhales.SSE;

/*
{
    "session_id":"305421642448879836",
    "udev_events":[
    {
        ".INPUT_CLASS":"joystick",
        "ACTION":"add",
        "CURRENT_TAGS":":seat:uaccess:",
        "DEVNAME":"/dev/input/event264",
        "DEVPATH":"/devices/virtual/input/input49/event264",
        "ID_INPUT":"1",
        "ID_INPUT_JOYSTICK":"1",
        "ID_SERIAL":"noserial",
        "MAJOR":"13",
        "MINOR":"264",
        "SEQNUM":"7",
        "SUBSYSTEM":"input",
        "TAGS":":seat:uaccess:",
        "USEC_INITIALIZED":"1773976267"
    },
    {
        ".INPUT_CLASS":"joystick",
        "ACTION":"add",
        "CURRENT_TAGS":":seat:uaccess:",
        "DEVNAME":"/dev/input/js265",
        "DEVPATH":"/devices/virtual/input/input49/js265",
        "ID_INPUT":"1",
        "ID_INPUT_JOYSTICK":"1",
        "ID_SERIAL":"noserial",
        "MAJOR":"13",
        "MINOR":"265",
        "SEQNUM":"7",
        "SUBSYSTEM":"input",
        "TAGS":":seat:uaccess:",
        "USEC_INITIALIZED":"1773976267"
    }],
    "udev_hw_db_entries":[["c13:264",["E:ID_INPUT=1","E:ID_INPUT_JOYSTICK=1","E:ID_BUS=usb","G:seat","G:uaccess","Q:seat","Q:uaccess","V:1"]]]
}
*/

[SseEventHandler]
public partial class PlugDeviceEventHandler : ISseEventHandler<string>
{
    public string EventName => "wolf::core::events::PlugDeviceEvent";
}