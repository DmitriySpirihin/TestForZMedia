using NUnit.Framework;
using UnityEngine;
using GameEnums;

[TestFixture]
public class GameLogicTests
{
    [Test]
    public void TakeDamageReturnZeroAsMinimumValue()
    {
        const float _damageAmount = 200f;
        float _currentHealth = 100f;

        float validDamage = Mathf.Max(_currentHealth - _damageAmount, 0f);
       Assert.AreEqual(0f, validDamage, 0.01f, "Minimal damage must be not less than zero");
    }
}
