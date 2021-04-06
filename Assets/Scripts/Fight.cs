using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Fight : MonoBehaviour
{
    [SerializeField]
    private Text _fightText;
    [SerializeField] private Button _nextTurn;
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
                   enemyO.atributes.Find(x => x is AgilityAtribute).ThrowCube();
        });
        
        Turn();
    }

    private void EndFight()
    {
        _order.Clear();
        _nextTurn.onClick.RemoveAllListeners();
        _nextTurn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        
        
    }
    private void Turn()
    {
        _nextTurn.onClick.RemoveAllListeners();
        _nextTurn.onClick.AddListener(Turn);
        foreach (var character in _order)
        {
            if (character.GetComponent<PlayerMovement>() != null)
            {
                var dmg = _playerAtributes.Damage();
                var dmgStr = dmg == 0 ? " промахнулся по " : " нанёс " + dmg.ToString();
                if (_enemyAtributes.ChangeHealth(-dmg))
                {
                    _fightText.text += _playerAtributes +  dmgStr + _enemyAtributes + "\n" + _enemyAtributes + " повержен";
                    EndFight();
                    return;
                }
                else
                {
                    _fightText.text += _playerAtributes + dmgStr + " " + _enemyAtributes + "\n";
                }
                
            }
            else
            {
                var dmg = _enemyAtributes.Damage();
                var dmgStr = dmg == 0 ? " промахнулся по " : " нанёс " + dmg.ToString();
                if(_playerAtributes.ChangeHealth(-dmg))
                {
                    _fightText.text += _enemyAtributes +  dmgStr + " " + _playerAtributes + "\n" + _playerAtributes + " повержен";
                    EndFight();
                    return;
                }
                else
                {
                     _fightText.text += _enemyAtributes +  dmgStr + " " + _playerAtributes + "\n";
                }
               
            }
        }
    }
}
