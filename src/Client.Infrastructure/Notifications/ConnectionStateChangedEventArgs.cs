namespace FSH.BlazorWebAssembly.Client.Infrastructure.Notifications;

public record ConnectionStateChangedEventArgs(ConnectionState State, string? Message);