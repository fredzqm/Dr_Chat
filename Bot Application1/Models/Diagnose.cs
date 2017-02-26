using System.Collections.Generic;

namespace Bot_Application1.Models
{
    public class Issue
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Accuracy { get; set; }
        public string Icd { get; set; }
        public string IcdName { get; set; }
        public string ProfName { get; set; }
        public int Ranking { get; set; }
    }

    public class Specialisation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SpecialistID { get; set; }
    }

    public class Diagnose
    {
        public Issue Issue { get; set; }
        public List<Specialisation> Specialisation { get; set; }
    }
}