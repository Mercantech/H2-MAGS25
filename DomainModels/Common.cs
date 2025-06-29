using System;

public abstract class Common
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected Common()
    {
        Id = Guid.NewGuid().ToString();
    }
}
