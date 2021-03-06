using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ClassData
{
    public ClassData() { }
    public ClassData(string _classInstructor, string _className, string _classId, string _classNumber, 
        string _firstDayOfWeek, string _firstStartTime, string _firstEndTime, string _secondDayOfWeek, string _secondStartTime, string _secondEndTime, object _students)
    {
        classInstructor = _classInstructor;

        className = _className;

        classId = _classId;
        classNumber = _classNumber;

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

    public string classId;
    public string classNumber;

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