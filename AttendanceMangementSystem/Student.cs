using System;

internal class Student
{
    static int _id = -1;
    public int ID { get; private set; }
    public string CollegiateID { get; set; }
    public string Name { get; set; }
    public string BluetoothAddress { get; set; }

    public Student()
    {
        ID = ++_id;
    }

    public override bool Equals(object obj)
    {
        return obj is Student student &&
               Name.Equals(student.Name, StringComparison.OrdinalIgnoreCase) &&
               BluetoothAddress.Equals(student.BluetoothAddress, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, BluetoothAddress);
    }
}
