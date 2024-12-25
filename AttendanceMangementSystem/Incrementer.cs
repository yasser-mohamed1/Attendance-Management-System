namespace AttendanceMangementSystem
{
    internal class Incrementer
    {
        static int _address = 0;
        public int Address => ++_address;
    }
}
