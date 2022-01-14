namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public interface IAddEditModal
{
    void ForceRender();
    bool Validate(object request);
}