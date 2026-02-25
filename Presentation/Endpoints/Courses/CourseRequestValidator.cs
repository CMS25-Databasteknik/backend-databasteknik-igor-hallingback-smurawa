namespace Backend.Presentation.API.Endpoints.Courses;

public static class CourseRequestValidator
{
    public static Dictionary<string, string[]> Validate(string title, string description, int durationInDays)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(title))
            errors["title"] = ["title is required."];

        if (string.IsNullOrWhiteSpace(description))
            errors["description"] = ["description is required."];

        if (durationInDays <= 0)
            errors["durationInDays"] = ["durationInDays must be greater than zero."];

        return errors;
    }
}
