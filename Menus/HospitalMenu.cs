using PulseCare.Models.Doctor;
using PulseCare.Models.Hospital;
using PulseCare.Models.Nurse;
using PulseCare.Models.Patient;
using PulseCare.Services;
using Spectre.Console;

namespace PulseCare.Menus
{
    internal class HospitalMenu
    {
        private readonly HospitalService _hospitalService;
        private readonly DoctorService doctorService;
        private readonly NurseService _nurseService;
        private readonly PatientService patientService;

        public HospitalMenu(HospitalService hospitalService, DoctorService doctorService, NurseService nurseService, PatientService patientService)
        {
            _hospitalService = hospitalService;
            _nurseService = nurseService;
            this.doctorService = doctorService;
            this.patientService = patientService;
        }

        public void Run(int hospitalId)
        {
            AnsiConsole.Clear();
            var rule = new Rule("\n[red]Hospital Menu[/]");
            AnsiConsole.Write(rule);
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] { "Get Hospital Details", "Get Doctors in Hospital", "Get Nurses in Hospital", "Get Patients in Hospital", "Exit" }));

            switch (choice)
            {
                case "Get Hospital Details":
                    AnsiConsole.Clear();
                    GetHospitalDetails(hospitalId);
                    break;
                case "Get Doctors in Hospital":
                    AnsiConsole.Clear();
                    GetDoctorsInHospital(hospitalId);
                    break;
                case "Get Nurses in Hospital":
                    AnsiConsole.Clear();
                    GetNursesInHospital(hospitalId);
                    break;
                case "Get Patients in Hospital":
                    AnsiConsole.Clear();
                    GetPatientsInHospital(hospitalId);
                    break;
                case "Exit":
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Exiting Hospital Menu.");
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }

            Run(hospitalId);
        }

        private void GetHospitalDetails(int hospitalId)
        {
            try
            {
                var hospital = _hospitalService.GetByIdAsync(hospitalId).Result;
                DisplayHospitalDetails(hospital);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void DisplayHospitalDetails(HospitalViewModel hospital)
        {
            try
            {
                AnsiConsole.WriteLine("Hospital Details");
                var table = new Table();
                table.AddColumn("Property").AddColumn("Value");
                table.AddRow("ID", hospital.Id.ToString());
                table.AddRow("Name", hospital.Name);
                table.AddRow("Balance", hospital.Balance.ToString());
                table.AddRow("Rooms Count", hospital.RoomsCount.ToString());
                AnsiConsole.Write(table);

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void GetDoctorsInHospital(int hospitalId)
        {
            try
            {
                var doctorsInHospital = doctorService.GetAllDoctorsAsync().Result;
                DisplayDoctors(doctorsInHospital);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void DisplayDoctors(IEnumerable<DoctorViewModel> doctors)
        {
            try
            {
                AnsiConsole.WriteLine("Doctors in Hospital");
                var table = new Table().RoundedBorder();
                table.AddColumn("ID").AddColumn("First Name").AddColumn("Last Name").AddColumn("Phone Number").AddColumn("Salary").AddColumn("Specialization");

                foreach (var doctor in doctors)
                {
                    table.AddRow(doctor.Id.ToString(), doctor.FirstName, doctor.LastName, doctor.PhoneNumber, doctor.Salary.ToString(), doctor.Specialization.ToString());
                }

                AnsiConsole.Write(table);

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void GetNursesInHospital(int hospitalId)
        {
            try
            {
                var nursesInHospital = _nurseService.GetAllNursesAsync().Result;
                DisplayNurses(nursesInHospital);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void DisplayNurses(IEnumerable<NurseViewModel> nurses)
        {
            try
            {
                AnsiConsole.WriteLine("Nurses in Hospital");
                var table = new Table().RoundedBorder();
                table.AddColumn("ID").AddColumn("First Name").AddColumn("Last Name").AddColumn("Phone Number").AddColumn("Salary").AddColumn("Assigned Doctor ID").AddColumn("Assigned Patient ID");

                foreach (var nurse in nurses)
                {
                    table.AddRow(nurse.Id.ToString(), nurse.FirstName, nurse.LastName, nurse.PhoneNumber, nurse.Salary.ToString(), nurse.AssignedDoctorId.ToString(), nurse.AssignedPatientId.ToString());
                }

                AnsiConsole.Write(table);

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void GetPatientsInHospital(int hospitalId)
        {
            try
            {
                var patientsInHospital = patientService.GetAllPatientsAsync().Result;
                DisplayPatients(patientsInHospital);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void DisplayPatients(IEnumerable<PatientViewModel> patients)
        {
            try
            {
                AnsiConsole.WriteLine("Patients in Hospital");
                var table = new Table().RoundedBorder();
                table.AddColumn("ID").AddColumn("First Name").AddColumn("Last Name").AddColumn("Gender").AddColumn("Date of Birth").AddColumn("Phone Number").AddColumn("Email").AddColumn("Address").AddColumn("Balance").AddColumn("Room Number").AddColumn("Assigned Doctor ID").AddColumn("Assigned Nurse ID").AddColumn("Status");

                foreach (var patient in patients)
                {
                    table.AddRow(patient.Id.ToString(), patient.FirstName, patient.LastName, patient.Gender.ToString(), patient.DateOfBirth.ToString(), patient.PhoneNumber, patient.Email, patient.Address, patient.Balance.ToString(), patient.AssignedDoctorId.ToString(), patient.AssignedNurseId.ToString(), patient.Status.ToString());
                }

                AnsiConsole.Write(table);

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }
    }
}
