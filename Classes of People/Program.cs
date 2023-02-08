using System;

namespace Classes_of_People
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Greetings.Person person = new Greetings.Person("Akiro", 0123456789, "Akiro@Email");
            Greetings.Doctor doctor = new Greetings.Doctor("Eric", 987654321, "Eric@Email", 500000);
            Greetings.Boss boss = new Greetings.Boss("John", 401787234, "John@Email", 1000000, 9.5f);

            person.SayGreeting();
            person.GiveDetails();

            //Changing details
            person.EmailAddress("Different@Email");
            person.PhoneNumber(450873489);

            // Print out new details
            person.SayChangedDetails();

            doctor.SayGreeting();
            doctor.GiveDetails();
            doctor.SaySalary();

            boss.SayGreeting();
            boss.GiveDetails();
            boss.SaySalary();
            boss.SayHoursWorking();



            Console.ReadKey();
        }
    }
}

namespace Greetings
{
    class Person
    {
        protected string name;
        protected int phoneNumber;
        protected string emailAddress;

        public Person(string name, int phoneNumber, string emailAddress) {
            this.name = name;
            this.phoneNumber = phoneNumber;
            this.emailAddress = emailAddress;
        }

        public virtual void SayGreeting() {
            Console.WriteLine($"Hello, I'm {name}");
        }

        public void SayChangedDetails() {
            Console.WriteLine($"I just changed my details to {emailAddress} and {phoneNumber}");
        }

        public void GiveDetails() {
            Console.WriteLine($"My email address is {emailAddress} and my phone number is {phoneNumber}");
        }

        public void Name(string name) {
            this.name = name;
        }

        public void PhoneNumber(int phoneNumber) {
            this.phoneNumber = phoneNumber;
        }

        public void EmailAddress(string emailAddress) {
            this.emailAddress = emailAddress;
        }
    }

    class Doctor : Person
    {
        protected float salary;

        public Doctor(string name, int phoneNumber, string emailAddress, float salary) 
            // Provide the base class constructor the required parameters
            : base(name, phoneNumber, emailAddress) {

            this.name = name;
            this.phoneNumber = phoneNumber;
            this.emailAddress = emailAddress;
            this.salary = salary;
        }

        public virtual void SaySalary() {
            Console.WriteLine($"My salary is ${salary}");
        }

        public override void SayGreeting() {
            Console.WriteLine($"\nHello, I'm Dr. {name}");
        }

        public void Salary(int salary) {
            this.salary = salary;
        }
    }

    class Boss : Doctor
    {
        protected float hoursWorking;

        public Boss(string name, int phoneNumber, string emailAddress, float salary, float hoursWorking)
            // Provide the base class constructor the required parameters
            : base(name, phoneNumber, emailAddress, salary) {

            this.name = name;
            this.phoneNumber = phoneNumber;
            this.emailAddress = emailAddress;
            this.salary = salary;
            this.hoursWorking = hoursWorking;
        }

        public void HoursWorking(float hoursWorking) {
            this.hoursWorking = hoursWorking;
        }

        public override void SayGreeting() {
            Console.WriteLine($"\nHello, I'm Dr. {name} and I'm the Boss but I'm also a doctor");
        }

        public override void SaySalary() {
            Console.WriteLine($"My salary is ${salary}");
        }

        public void SayHoursWorking() {
            Console.WriteLine($"I work {hoursWorking} hours a day");
        }
    }
}
