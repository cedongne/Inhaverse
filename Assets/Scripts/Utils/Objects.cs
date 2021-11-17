using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ClassData
{
    public ClassData() { }
    public ClassData(string _classInstructor, string _className, string _classLateCheckTime, string _classId, string _classNumber, string _classEnterAllowTime, 
        string _firstDayOfWeek, string _firstStartTime, string _firstEndTime, string _secondDayOfWeek, string _secondStartTime, string _secondEndTime, object _students)
    {
        classInstructor = _classInstructor;

        className = _className;
        classLateCheckTime = _classLateCheckTime;

        classId = _classId;
        classNumber = _classNumber;
        classEnterAllowTime = _classEnterAllowTime;

        firstDayOfWeek = _firstDayOfWeek;
        firstStartTime = _firstStartTime;
        firstEndTime = _firstEndTime;

        secondDayOfWeek = _secondDayOfWeek;
        secondStartTime = _secondStartTime;
        secondEndTime = _secondEndTime;

        students = _students as List<UserInfo>;
    }
    public string classInstructor;

    public string className;
    public string classLateCheckTime;

    public string classId;
    public string classNumber;
    public string classEnterAllowTime;

    public string firstDayOfWeek;
    public string firstStartTime;
    public string firstEndTime;

    public string secondDayOfWeek;
    public string secondStartTime;
    public string secondEndTime;

    public List<UserInfo> students;
}

[System.Serializable]
public class ClassList
{
    public ClassList() { }
    public ClassList(int _classCount, GameObject _button, string _entityId, string _entityType)
    {
        classCount = _classCount;
        button = _button;
        entityId = _entityId;
        entityType = _entityType;
    }
    public int classCount;
    public GameObject button;
    public string entityId;
    public string entityType;
}

[System.Serializable]
public class UserInfo
{
    public UserInfo() { }
    public UserInfo(string _schoolId, string _name)
    {
        schoolId = _schoolId;
        name = _name;
    }
    public string schoolId;
    public string name;
}

[System.Serializable]
public class AttendanceTable{
    public AttendanceTable() {
        attendanceInfo = new Dictionary<string, Define.ATTENDANCE[]>();
    }

    public AttendanceTable(string className, Define.ATTENDANCE attendance)
    {
        if(attendanceInfo == null)
            attendanceInfo = new Dictionary<string, Define.ATTENDANCE[]>();
        if (!attendanceInfo.ContainsKey(className))
        {
            Add(className, attendance);
        }
        else
        {
        }

//        else
  //          attendanceInfo[className][] = attendance;
    }

    public UserInfo studentInfo;
    public Dictionary<string, Define.ATTENDANCE[]> attendanceInfo;
    
    public void Add(string className, Define.ATTENDANCE attendance)
    {
        attendanceInfo.Add(className, new Define.ATTENDANCE[32]);
        Attend(className, attendance);
    }

    public void Attend(string className, Define.ATTENDANCE attendance)
    {
        int classDay = (UtilityMethods.GetWeekOfSemester() - 1) * 2;
        if (!attendanceInfo[className][classDay].Equals(Define.ATTENDANCE.YET))
        {

        }
        attendanceInfo[className][UtilityMethods.GetWeekOfSemester() * 2] = attendance;
    }
    
}