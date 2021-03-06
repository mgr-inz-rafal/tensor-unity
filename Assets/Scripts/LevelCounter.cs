public class LevelCounter {
    int currentLevel;
    bool reachedFinal;

    public LevelCounter() {
        currentLevel = 1;
        reachedFinal = false;
    }

    public int Get() {
        return currentLevel;
    }

    public void Set(int level) {
        if (level == 1) {
            ResetFinal();
        }
        currentLevel = level;
    }

    public bool IsLast() {
        return currentLevel == Consts.MAX_LEVEL_NUMBER + 1;
    }

    public bool ReachedFinal() {
        return reachedFinal;
    }

    public void ResetFinal() {
        reachedFinal = false;
    }

    public void Decrease() {
        currentLevel--;
        if (0 == currentLevel)
        {
            currentLevel = Consts.MAX_LEVEL_NUMBER;
        }
    }

    public void Increase(bool fromWinningLevel) {
        currentLevel++;
        if (fromWinningLevel && IsLast())
        {
            reachedFinal = true;
        }
        else
        {
            reachedFinal = false;
        }

        if (IsLast()) {
            currentLevel = 1;
        }
    }
};