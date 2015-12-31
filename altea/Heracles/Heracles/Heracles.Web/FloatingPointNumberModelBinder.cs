using System;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

public class FloatingPointNumberModelBinder : DefaultModelBinder
{
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(float)
            && bindingContext.ModelType != typeof(double)
            && bindingContext.ModelType != typeof(decimal))
        {
            return base.BindModel(controllerContext, bindingContext);
        }

        ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (result == null || string.IsNullOrEmpty(result.AttemptedValue))
        {
            return base.BindModel(controllerContext, bindingContext);
        }

        object[] parameters = new object[] { result.AttemptedValue, NumberStyles.Number, CultureInfo.InvariantCulture, null };

        bool status = (bool)bindingContext.ModelType.InvokeMember(
            "TryParse",
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
            Type.DefaultBinder,
            null,
            parameters);

        return status ? parameters[3] : base.BindModel(controllerContext, bindingContext);
    }
}