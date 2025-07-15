
namespace HorsesForCourses.Core
{
    public class Course(string title, Period period)
    {
        public string Title { get; } = title;
        public Period Period { get; } = period;
        private readonly HashSet<string> _requiredSkills = new(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyCollection<string> RequiredSkills => _requiredSkills;
        private readonly List<TimeSlot> _schedule = [];
        public IReadOnlyCollection<TimeSlot> Schedule => _schedule.AsReadOnly();
        public bool IsConfirmed { get; private set; } = false;
        public Coach? AssignedCoach { get; private set; } = null;


        public void Confirm()
        {
            if (!_schedule.Any()) throw new InvalidOperationException("Cannot confirm course without any lessons.");

            IsConfirmed = true;
        }

        private void ConfirmConfrimation()
        {
            if (IsConfirmed) throw new InvalidOperationException("Cannot modify course after it has been confirmed.");
        }

        public void AddRequiredSkill(string skill)
        {
            ConfirmConfrimation();
            _requiredSkills.Add(skill);
        }

        public void RemoveRequiredSkill(string skill)
        {
            ConfirmConfrimation();
            _requiredSkills.Remove(skill);
        }

        public void AddTimeSlot(TimeSlot timeSlot)
        {
            ConfirmConfrimation();
            _schedule.Add(timeSlot);
        }

        public void RemoveTimeSlot(TimeSlot timeSlot)
        {
            ConfirmConfrimation();
            _schedule.Remove(timeSlot);
        }

        public void AssignCoach(Coach coach)
        {
            if (!IsConfirmed) throw new InvalidOperationException("Course must be confirmed before assigning a coach.");

            if (!coach.HasAllSkills(_requiredSkills)) throw new InvalidOperationException("Coach does not have all required skills.");

            if (coach.IsAvailableCoach()) AssignedCoach = coach;

            
        }
    }
}
