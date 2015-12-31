namespace AlteaLabs.Portal.Contracts.Commands
{
    public class RegisterUser : ChangeUserPassword
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        
    }
}
