public class Define
{
    public enum UI
    {
        LOGIN,
        HUD,
        PORTRAIT,
        MENU,
        CLASS,
        CLASSMAKING,
        CLASSLIST,
        CONFERENCE,
        PLAYERINFO,
        BOARD,
        OPENFILE,
        CLASSSTUDENTLIST,
        OPTION,
        COMMAND,
        CLASSREADY,
        CLASSCHAIR,
        SCREENSHOT
    }

    public enum GROUPLISTUSING
    {
        MAKEBUTTONS,
        GETGROUPNAMES,
        FINDSPECIFICGROUP
    }

    public enum USERDATAUSING
    {
        JOINTOCLASS,
        LOADCLASSINFO
    }

    public enum ATTENDANCE
    {
        ATTENDANCE,
        LATE,
        ABSENT,
        YET
    }

    public enum CLASSSTATE
    {
        READY,
        START,
        END
    }

    public enum VIDEOCONFERENCESTATE
    {
        READY,
        START,
        END
    }
}
