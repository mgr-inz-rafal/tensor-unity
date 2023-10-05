class Consts {
    public const int MOVEMENT_WARMUP = 25;
    public const int TOTAL_ELEVATOR_FRAMES = (int)(50 * 1.5f);
    public const int LEVEL_DIMENSION = 12;
    public const int MAXIMUM_AMYGDALAS_TO_ACTIVATE = Consts.LEVEL_DIMENSION * 2; // No more than two columns will ever be activated
    public const int ROTATION_STEP = 5;
    public const int MAX_LEVEL_NUMBER = 58;

    public static readonly float[] CAMERA_DISTANCE_TABLE = {
        -10f,
        -10.52f,
        -11.03f,
        -11.5f,
        -11.93f,
        -12.3f,
        -12.6f,
        -12.82f,
        -12.95f,
        -13f,
        -12.95f,
        -12.82f,
        -12.6f,
        -12.3f,
        -11.93f,
        -11.5f,
        -11.03f,
        -10.52f,
        -10f
    };
}