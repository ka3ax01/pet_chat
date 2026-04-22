namespace WhatsappClone.Application.Auth.Register;

public sealed record RegisterCommand(string UserName, string Email, string Password);
