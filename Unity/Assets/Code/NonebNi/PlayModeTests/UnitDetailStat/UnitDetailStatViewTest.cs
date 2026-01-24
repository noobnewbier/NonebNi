/*
 * TODO: Note why are you keeping this
 * This is just a way to keep track of how you used to set up ui automated test
 * if it's bothering you too much just yeet it
 */

// using System;
// using System.Collections;
// using JetBrains.Annotations;
// using NonebNi.Core.Actions;
// using NonebNi.Core.Units;
// using NonebNi.Ui.Statistics.Skill;
// using NonebNi.Ui.Statistics.Unit;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.TestTools;
// using UnityUtils.TestUtils;
// using Object = UnityEngine.Object;
//
// namespace NonebNi.PlayModeTests.UnitDetailStat
// {
//     public class UnitDetailStatViewTest
//     {
//         private readonly Sprite _testSprite = Sprite.Create(
//             Texture2D.whiteTexture,
//             new Rect(
//                 1f,
//                 1f,
//                 1f,
//                 1f
//             ),
//             Vector2.zero
//         );
//
//         private Ui.Statistics.Unit.UnitDetailStat _detailStat = null!;
//
//         private UnitData _testData = null!;
//         private UnitDetailStatView _unitDetailStatView = null!;
//
//         [UnitySetUp]
//         [UsedImplicitly]
//         public IEnumerator SetUp()
//         {
//             const string? sceneName = "UnitDetailStatViewTestScene";
//             SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
//
//             var waitForSceneLoaded = new WaitForSceneLoaded(sceneName);
//             yield return waitForSceneLoaded;
//
//
//             _testData = new UnitData(
//                 Guid.Empty,
//                 new[]
//                 {
//                     new NonebAction("NoobSkill1", a1, 1, 1, TargetArea.Single, TargetRestriction.Enemy),
//                     new NonebAction("NoobSkill2", 1, 1, 1, TargetArea.Single, TargetRestriction.Enemy)
//                 },
//                 _testSprite,
//                 "Noob",
//                 "TestFaction",
//                 100,
//                 200,
//                 0,
//                 5,
//                 0,
//                 0,
//                 0,
//                 0,
//                 0,
//                 0,
//                 0,
//                 1
//             );
//
//             _detailStat = Object.FindObjectOfType<Ui.Statistics.Unit.UnitDetailStat>();
//             _detailStat.Init();
//             _unitDetailStatView = _detailStat.statView;
//         }
//
//         [UnityTest]
//         public IEnumerator UnitDetailStatViewTestSimplePasses()
//         {
//             yield return _unitDetailStatView.Show(_testData);
//
//             Assert.AreSame(
//                 _detailStat.image.sprite,
//                 _testData.Icon,
//                 "The image sprite should be the same with the unit data's icon"
//             );
//             Assert.AreEqual(
//                 _detailStat.skillPanelRoot.GetComponentsInChildren<SkillView>().Length,
//                 _testData.Actions.Length,
//                 "The number of skill views should match the number of skills the unit has"
//             );
//         }
//     }
// }

