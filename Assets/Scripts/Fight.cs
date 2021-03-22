using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Fight : MonoBehaviour
{
    public Text FightText;
    private GameObject _player;
    private GameObject _enemy;
    private Atributes _playerAtributes;
    private Enemy _enemyAtributes;
    private List<GameObject> _order;

    public void BeginFight(GameObject player, GameObject enemy)
    {
        _player = player;
        _enemy = enemy;
        _playerAtributes = player.GetComponent<Atributes>();
        _enemyAtributes = enemy.GetComponent<Enemy>();
        _order = new List<GameObject>() {_player, _enemy};
        _order.Sort((o, o1) =>
        {
            var playerO = o.GetComponent<Atributes>();
            var enemyO = o1.GetComponent<Enemy>();
            return playerO.PlayerAtributes.Find(x => x is AgilityAtribute).ThrowCube() -
                   enemyO.Atributes.Find(x => x is AgilityAtribute).ThrowCube();
        });
        
        gameObject.GetComponentInChildren<Button>().onClick.AddListener(Turn);
    }

    private void EndFight()
    {
        _order.Clear();
        gameObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        
        
    }
    private void Turn()
    {
        foreach (var character in _order)
        {
            if (character.GetComponent<PlayerMovement>() != null)
            {
                var dmg = _playerAtributes.Damage();
                var dmgStr = dmg == 0 ? " промахнулся по " : " нанёс " + dmg.ToString();
                if (_enemyAtributes.ChangeHealth(-dmg))
                {
                    FightText.text += _playerAtributes +  dmgStr + _enemyAtributes + "\n" + _enemyAtributes + " повержен";
                    EndFight();
                    return;
                }
                else
                {
                    FightText.text += _playerAtributes + dmgStr + " " + _enemyAtributes + "\n";
                }
                
            }
            else
            {
                var dmg = _enemyAtributes.Damage();
                var dmgStr = dmg == 0 ? " промахнулся по " : " нанёс " + dmg.ToString();
                if(_playerAtributes.ChangeHealth(-dmg))
                {
                    FightText.text += _enemyAtributes +  dmgStr + " " + _playerAtributes + "\n" + _playerAtributes + " повержен";
                    EndFight();
                    return;
                }
                else
                {
                     FightText.text += _enemyAtributes +  dmgStr + " " + _playerAtributes + "\n";
                }
               
            }
        }
    }
}
