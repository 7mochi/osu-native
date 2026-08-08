using osu.Native.Objects;
using osu.Native.Objects.Difficulty;
using osu.Native.Objects.Performance;
using osu.Native.Structures;
using osu.Native.Structures.Difficulty;
using osu.Native.Structures.Performance;

namespace osu.Native.Tests.Objects.Performance;

internal unsafe class ManiaPerformanceCalculatorTests
{
    private NativeRuleset _nativeRuleset;
    private NativeBeatmap _nativeBeatmap;
    private NativeManiaPerformanceCalculator _nativePerformanceCalculator;

    [SetUp]
    public void Setup()
    {
        fixed (NativeRuleset* ptr = &_nativeRuleset)
            RulesetObject.CreateFromId(3, ptr);

        _nativeBeatmap = TestUtils.CreateBeatmap("beatmaps/mania/xi - FREEDOM DiVE (razlteh) [Blocko's 7K Black Another].osu");

        fixed (NativeManiaPerformanceCalculator* ptr = &_nativePerformanceCalculator)
            ManiaPerformanceCalculatorObject.Create(ptr);
    }

    /// <summary>
    /// Creates a performance calculator and expects Success to return.
    /// </summary>
    [Test]
    public void Create_Success()
    {
        NativeOsuPerformanceCalculator nativePerformanceCalculator;
        ErrorCode errorCode = OsuPerformanceCalculatorObject.Create(&nativePerformanceCalculator);

        Assert.That(errorCode, Is.EqualTo(ErrorCode.Success));
    }

    /// <summary>
    /// Creates a performance calculator, performs performance calculation for the specified score and expects the attributes to match the provided ones.
    /// </summary>
    [TestCaseSource(nameof(GetTestCases))]
    public void Calculate_Success(string beatmapFilename, string? mods, NativeScoreInfo scoreInfo, NativeManiaPerformanceAttributes expectedAttributes)
    {
        NativeBeatmap nativeBeatmap = TestUtils.CreateBeatmap(beatmapFilename);
        NativeModsCollection nativeModsCollection = TestUtils.CreateNativeModsCollection(mods);

        scoreInfo.RulesetHandle = _nativeRuleset.Handle;
        scoreInfo.BeatmapHandle = nativeBeatmap.Handle;
        scoreInfo.ModsHandle = nativeModsCollection.Handle;

        NativeManiaDifficultyCalculator nativeDifficultyCalculator;
        ManiaDifficultyCalculatorObject.Create(_nativeRuleset.Handle, nativeBeatmap.Handle, &nativeDifficultyCalculator);

        NativeManiaDifficultyAttributes nativeDifficultyAttributes;
        ManiaDifficultyCalculatorObject.Calculate(nativeDifficultyCalculator.Handle, nativeModsCollection.Handle, &nativeDifficultyAttributes);

        NativeManiaPerformanceAttributes nativeAttributes;
        ErrorCode errorCode = ManiaPerformanceCalculatorObject.Calculate(
            _nativePerformanceCalculator.Handle, scoreInfo, nativeDifficultyAttributes, &nativeAttributes);

        Assert.That(errorCode, Is.EqualTo(ErrorCode.Success));
        TestUtils.AssertEqualAttributes(nativeAttributes, expectedAttributes);
    }

    private static IEnumerable<TestCaseData> GetTestCases()
    {
        yield return new(
        "beatmaps/osu/Kenji Ninuma - DISCOPRINCE (peppy) [Normal].osu",
        null,
        new NativeScoreInfo()
        {
            Accuracy = 1,
            CountPerfect = 341,
        },
        new NativeManiaPerformanceAttributes(new()
        {
            Total = 29.360109024870148,
            Difficulty = 29.360109024870148,
        })
        );
        yield return new(
        "beatmaps/mania/xi - FREEDOM DiVE (razlteh) [Blocko's 7K Black Another].osu",
        "HRFI",
        new NativeScoreInfo()
        {
            Accuracy = 0.9974475699286899,
            CountMiss = 4,
            CountMeh = 3,
            CountOk = 12,
            CountGood = 7,
            CountGreat = 31,
            CountPerfect = 6796,
        },
        new NativeManiaPerformanceAttributes(new()
        {
            Total = 887.0030883091539,
            Difficulty = 887.0030883091539,
        })
        );
        yield return new(
        "beatmaps/mania/MYTH & ROID - STYX HELIX (Tsukuyomi) [victorica's Hard].osu",
        "HDDT",
        new NativeScoreInfo()
        {
            Accuracy = 0.8595799775327964,
            CountMiss = 34,
            CountMeh = 8,
            CountOk = 96,
            CountGood = 40,
            CountGreat = 117,
            CountPerfect = 566,
        },
        new NativeManiaPerformanceAttributes(new()
        {
            Total = 43.58112219618832,
            Difficulty = 43.58112219618832,
        })
        );
        yield return new(
        "beatmaps/mania/Soleily - Renatus (ExPew) [Another].osu",
        "INFL",
        new NativeScoreInfo()
        {
            Accuracy = 0.9994634414235846,
            CountMiss = 1,
            CountOk = 2,
            CountGood = 1,
            CountGreat = 6,
            CountPerfect = 5184,
        },
        new NativeManiaPerformanceAttributes(new()
        {
            Total = 604.5659672217433,
            Difficulty = 604.5659672217433,
        })
        );
    }
}
