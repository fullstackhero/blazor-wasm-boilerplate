namespace FL_CRMS_ERP_WASM.Client.Components.EntityTable;

public interface IAddEditModal<TRequest>
{
    TRequest RequestModel { get; }
    bool IsCreate { get; }
    void ForceRender();
}