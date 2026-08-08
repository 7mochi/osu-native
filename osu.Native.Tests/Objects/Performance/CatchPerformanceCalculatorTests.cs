using osu.Native.Objects;
using osu.Native.Objects.Difficulty;
using osu.Native.Objects.Performance;
using osu.Native.Structures;
using osu.Native.Structures.Difficulty;
using osu.Native.Structures.Performance;

namespace osu.Native.Tests.Objects.Performance;

internal unsafe class CatchPerformanceCalculatorTests
{
    private NativeRuleset _nativeRuleset;
    private NativeBeatmap _nativeBeatmap;
    private NativeCatchPerformanceCalculator _nativePerformanceCalculator;

    [SetUp]
    public void Setup()
    {
        fixed (NativeRuleset* ptr = &_nativeRuleset)
            RulesetObject.CreateFromId(2, ptr);

        _nativeBeatmap = TestUtils.CreateBeatmap("beatmaps/catch/Lite Show Magic (t+pazolite vs C-Show) - Crack Traxxxx (Fatfan Kolek) [Spec's Hi-Speed Overdose].osu");

        fixed (NativeCatchPerformanceCalculator* ptr = &_nativePerformanceCalculator)
            CatchPerformanceCalculatorObject.Create(ptr);
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
    public void Calculate_Success(string beatmapFilename, string? mods, NativeScoreInfo scoreInfo, NativeCatchPerformanceAttributes expectedAttributes)
    {
        NativeBeatmap nativeBeatmap = TestUtils.CreateBeatmap(beatmapFilename);
        NativeModsCollection nativeModsCollection = TestUtils.CreateNativeModsCollection(mods);

        scoreInfo.RulesetHandle = _nativeRuleset.Handle;
        scoreInfo.BeatmapHandle = nativeBeatmap.Handle;
        scoreInfo.ModsHandle = nativeModsCollection.Handle;

        NativeCatchDifficultyCalculator nativeDifficultyCalculator;
        CatchDifficultyCalculatorObject.Create(_nativeRuleset.Handle, nativeBeatmap.Handle, &nativeDifficultyCalculator);

        NativeCatchDifficultyAttributes nativeDifficultyAttributes;
        CatchDifficultyCalculatorObject.Calculate(nativeDifficultyCalculator.Handle, nativeModsCollection.Handle, &nativeDifficultyAttributes);

        NativeCatchPerformanceAttributes nativeAttributes;
        ErrorCode errorCode = CatchPerformanceCalculatorObject.Calculate(
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
                MaxCombo = 310,
                Accuracy = 1,
                CountGreat = 235,
                CountSmallTickHit = 360,
                CountLargeTickHit = 75,
            },
            new NativeCatchPerformanceAttributes(new()
            {
                Total = 16.62329862571729,
            })
        );
        yield return new(
            "beatmaps/catch/Lite Show Magic (t+pazolite vs C-Show) - Crack Traxxxx (Fatfan Kolek) [Spec's Hi-Speed Overdose].osu",
            "DTFL",
            new NativeScoreInfo()
            {
                MaxCombo = 519,
                Accuracy = 0.8962892483349191,
                CountMiss = 17,
                CountGreat = 892,
                CountSmallTickMiss = 92,
                CountSmallTickHit = 15,
                CountLargeTickHit = 35,
            },
            new NativeCatchPerformanceAttributes(new()
            {
                Total = 305.6862748958652,
            })
        );
        yield return new(
            "beatmaps/catch/Hanatan - Airman ga Taosenai (SOUND HOLIC Ver.) (Natsu) [Zero's Overdose].osu",
            "FFEZ",
            new NativeScoreInfo()
            {
                MaxCombo = 924,
                Accuracy = 1,
                CountMiss = 2,
                CountGreat = 979,
                CountSmallTickMiss = -2,
                CountSmallTickHit = 244,
                CountLargeTickHit = 2,
            },
            new NativeCatchPerformanceAttributes(new()
            {
                Total = 250.05710106457926,
            })
        );
        yield return new(
            "beatmaps/catch/Icon For Hire - Make a Move (Speed Up Ver.) (Sotarks) [Ascendance's Overdose].osu",
            "MF",
            new NativeScoreInfo()
            {
                MaxCombo = 204,
                Accuracy = 0.7258064516129032,
                CountMiss = 3,
                CountGreat = 242,
                CountSmallTickMiss = 99,
                CountSmallTickHit = 11,
                CountLargeTickHit = 17,
            },
            new NativeCatchPerformanceAttributes(new()
            {
                Total = 31.724115302973036,
            })
        );
    }
}
