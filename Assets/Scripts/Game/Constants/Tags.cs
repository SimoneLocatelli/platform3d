public static class Tags
{
    // Materials
    public static readonly string Material_Wood = "Material_Wood";
    public static readonly string Material_Bone = "Material_Bone";

    public static readonly string Object = "Object";

    public static readonly string NPC = "NPC";

    public static class Collections
    {
        // Objects
        public static readonly string[] Object_Wood = new[] { Tags.Material_Wood,  Tags.Object  };

        // NPCs
        public static readonly string[] NPC_Skeleton = new[] { Tags.Material_Bone,  Tags.NPC };
    }
}