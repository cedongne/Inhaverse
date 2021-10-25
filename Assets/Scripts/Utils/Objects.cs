using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ClassData
{
    public ClassData() { }
    public ClassData(string cn, string ci, string fdw, string fst, string fet, string sdw, string sst, string set, object s)
    {
        className = cn;
        classId = ci;

        firstDayOfWeek = fdw;
        firstStartTime = fst;
        firstEndTime = fet;

        secondDayOfWeek = sdw;
        secondStartTime = sst;
        secondEndTime = set;

        students = s as List<StudentInfo>;
    }
    public string className;
    public string classId;

    public string firstDayOfWeek;
    public string firstStartTime;
    public string firstEndTime;

    public string secondDayOfWeek;
    public string secondStartTime;
    public string secondEndTime;

    public List<StudentInfo> students;
}

[System.Serializable]
public class ClassList
{
    public ClassList() { }
    public ClassList(int cc, GameObject b, string ei, string et)
    {
        classCount = cc;
        button = b;
        entityId = ei;
        entityType = et;
    }
    public int classCount;
    public GameObject button;
    public string entityId;
    public string entityType;
}

[System.Serializable]
public class StudentInfo
{
    public StudentInfo() { }
    public StudentInfo(string si, string sn)
    {
        studentId = si;
        studentName = sn;
    }
    public string studentId;
    public string studentName;
}