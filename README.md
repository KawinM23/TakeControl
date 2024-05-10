# TakeControl
2D Platformer Metroidvania for University's Project

## Game Control
- [WASD] to move
- [Spacebar] to Jump (Space again to Double Jump if the robot has the skill)
- [Shift] to Dash (if the robot has the skill)
- [Left Click] to Attack (follow the mouse)
- [Right Click] on the robot with indicator to TakeControl
- [Q] to Switch Weapon
- [R] to Reload the Gun
- [E] to Release Pulse Bomb (Delete all projectiles and knockback enemies)
- [ESC] to Pause

# Assets
- [Robots Sprites](https://mounirtohami.itch.io/26-animated-pixelart-robots)

# Developing Note

## Code Styles & Convention
In this project, we follow a set of code styles and conventions to ensure consistency and maintainability. Please adhere to the following guidelines when contributing to the codebase:

### 1. Naming Conventions
Use descriptive and meaningful names for variables, functions, and classes.
- Follow `camelCase` naming convention for `local variables`
```cs
void TakeDamage(int damage) {
    int someNumber = 123;
    ...
}
```
- Use `PascalCase` naming convention for `classes, components, methods, and public fields`
```cs
public bool IsHackable;
```
```cs
public class PlayerManager : MonoBehaviour {
    ...
}
```
```cs
void JumpAction() {
    ...
}
```
- Use `_camelCase` (with an underscore) for `private fields`
```cs
private bool _isActivatable;
```
### 2. Comments and Documentation
- Include comments to explain complex logic, algorithms, or any non-obvious code.
- ***Please allow group members to understand your code at a glance***

### 3. Inspector Settings
These are useful for our own developer experience. Please see previous code for reference.

Amazing Tip: `Ctrl+F`

- Use `[SerializeField]` attribute to expose private fields to the Unity Inspector.
```cs
[SerializeField] private float _speed;
```
- Use `[Header("...")]` attribute to group related fields in the Inspector.
```cs
[Header("Player Settings")]
[SerializeField] private float _speed;
...

[Header("Enemy Settings")]
[SerializeField] private float _damage;
...
```
- Use `[Tooltip("...")]` attribute to provide additional information about the field in the Inspector.
```cs
[Tooltip("The speed of the player")]
[SerializeField] private float _speed;
```
- Use `[Range(min, max)]` attribute to create a slider in the Inspector for numerical fields.
```cs
[Range(0, 100)]
[SerializeField] private float _health;
```
- Use `[Space]` attribute to add space between fields in the Inspector.
```cs
[Header("Player Settings")]
[SerializeField] private float _speed;

[Space(10)]
[SerializeField] private float _jumpForce;
```
