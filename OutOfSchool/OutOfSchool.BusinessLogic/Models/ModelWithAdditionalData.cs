﻿namespace OutOfSchool.BusinessLogic.Models;

public class ModelWithAdditionalData<TModel, TData>
    where TModel : class
{
    public string Description { get; set; } = string.Empty;

    public TModel Model { get; set; } = null;

    public TData AdditionalData { get; set; }
}