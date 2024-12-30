using System;

namespace Application.Commons.DTOs;

public class PostReactionCountDto
{
    public string? PostId { get; set; }
    public string? ShareId { get; set; }
    public Like? Like { get; set; }
    public Love? Love { get; set; }
    public Haha? Haha { get; set; }
    
    public Wow? Wow { get; set; }

    public Sad? Sad { get; set; }

    public Angry? Angry { get; set; }


    public int Total { get; set; }
}
public class Like {
    public int LikeCount { get; set; }
    public string? Icon { get; set; }
}

public class Love {
    public int LoveCount { get; set; }
    public string? Icon { get; set; }
}

public class Haha {
    public int HahaCount { get; set; }
    public string? Icon { get; set; }
}

public class Wow {
    public int WowCount { get; set; }
    public string? Icon { get; set; }
}


public class Sad {
    public int SadCount { get; set; }
    public string? Icon { get; set; }
}


public class Angry {
    public int AngryCount { get; set; }
    public string? Icon { get; set; }
}


