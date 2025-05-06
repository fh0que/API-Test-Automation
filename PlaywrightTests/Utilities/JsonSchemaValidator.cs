using System;
using System.Text.Json;
using Json.Schema;

public class JsonSchemaValidator
{
    public static bool Validate(string jsonResponse, string schemaJson, out string validationErrors)
    {
        try
        {
            var schema = JsonSchema.FromText(schemaJson);
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            var result = schema.Evaluate(jsonDocument.RootElement);

            if (result.IsValid)
            {
                validationErrors = string.Empty;
                return true;
            }
            else
            {
                validationErrors = string.Join("; ", result.Errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            validationErrors = $"Schema validation failed: {ex.Message}";
            return false;
        }
    }
}
