using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public record EntityField<TEntity>(string SortLabel, string DataLabel, Func<TEntity, object?> DataValue, RenderFragment<TEntity>? Template = null)
{
    /// <summary>
    /// The string that's sent to the api as property to sort on for this field.
    /// </summary>
    public string SortLabel { get; set; } = SortLabel;

    /// <summary>
    /// The string that's shown on the UI for this field.
    /// </summary>
    public string DataLabel { get; set; } = DataLabel;

    /// <summary>
    /// A function that returns the actual value of this field from the supplied entity.
    /// </summary>
    public Func<TEntity, object?> DataValue { get; set; } = DataValue;

    /// <summary>
    /// When supplied this template will be used for this field in stead of the default template.
    /// For an example on how to do this, see <see cref="Pages.Personal.AuditLogs"/>.
    /// </summary>
    public RenderFragment<TEntity>? Template { get; set; } = Template;
}