# 🎨 EXEMPLES VISUELS DE CONFIGURATION BEHAVIOR TREES

## 📋 CONVENTION DE NOTATION

```
[Type] Nom de la tâche (paramètres)
  ↳ Enfant 1
  ↳ Enfant 2

Types:
  ⚙️ = Composite (Sequence, Selector, Parallel)
  ✓ = Conditional (retourne Success/Failure)
  ⚡ = Action (effectue une action)
  🔄 = Decorator (Repeat, Inverter, etc.)
```

---

## 🔴 DRONE ROUGE (Sol) - Configuration Détaillée

### Architecture Hiérarchique:

```
🌳 Behavior Tree: DroneRedBehaviorAdvanced
│
└─ ⚙️ ROOT (Selector)
    │
    ├─ ⚙️ SEQUENCE: "1. Survival Mode" ⭐ PRIORITÉ 1
    │   ├─ ✓ CheckLowHealth
    │   │   └─ healthThreshold = 30.0
    │   ├─ ⚡ UnlockCurrentTarget
    │   └─ ⚡ TacticalRetreat
    │       ├─ retreatDistance = 20.0
    │       └─ target = [Shared: target]
    │
    ├─ ⚙️ SEQUENCE: "2. Combat Intelligent" ⭐ PRIORITÉ 2
    │   │
    │   ├─ ⚙️ SELECTOR: "Target Selection"
    │   │   │
    │   │   ├─ ⚙️ SEQUENCE: "Keep Weak Target"
    │   │   │   ├─ ✓ CheckTargetHealth
    │   │   │   │   ├─ target = [Shared: target]
    │   │   │   │   └─ healthThreshold = 40.0
    │   │   │   └─ ⚡ Success (node intégré)
    │   │   │
    │   │   ├─ ⚡ SelectWeakestEnemy "Priority: Turret"
    │   │   │   ├─ enemyType = "Turret"
    │   │   │   ├─ target = [Shared: target]
    │   │   │   ├─ minRadius = 0.0
    │   │   │   └─ maxRadius = 100.0
    │   │   │
    │   │   └─ ⚡ SelectWeakestEnemy "Fallback: Drone"
    │   │       ├─ enemyType = "Drone"
    │   │       ├─ target = [Shared: target]
    │   │       ├─ minRadius = 0.0
    │   │       └─ maxRadius = 100.0
    │   │
    │   ├─ ⚡ KeepDistance "Tactical Positioning"
    │   │   ├─ target = [Shared: target]
    │   │   ├─ optimalDistance = 15.0
    │   │   └─ distanceTolerance = 3.0
    │   │
    │   ├─ ✓ CheckTargetInRange "Engagement Check"
    │   │   ├─ target = [Shared: target]
    │   │   ├─ minRange = 0.0
    │   │   └─ maxRange = 25.0
    │   │
    │   ├─ ⚡ MySeek "Move to Target"
    │   │   ├─ target = [Shared: target]
    │   │   └─ arriveDistance = 2.0
    │   │
    │   ├─ ⚡ DroneShoot "Fire!"
    │   │
    │   └─ ⚡ WaitRandom "Cooldown"
    │       ├─ minWaitTime = 0.3
    │       └─ maxWaitTime = 0.8
    │
    └─ ⚙️ SEQUENCE: "3. Idle/Patrol" ⭐ PRIORITÉ 3
        └─ ⚡ WaitRandom
            ├─ minWaitTime = 1.0
            └─ maxWaitTime = 3.0
```

### Variables Partagées (Shared Variables):

```
Variable Name      | Type            | Is Shared | Value
-------------------|-----------------|-----------|-------
target             | Transform       | YES       | null
minRadius          | Float           | YES       | 0.0
maxRadius          | Float           | YES       | 100.0
healthThreshold    | Float           | NO        | 30.0
optimalDistance    | Float           | NO        | 15.0
retreatDistance    | Float           | NO        | 20.0
```

---

## 🚁 FLYING DRONE ROUGE - Configuration Avancée

### Architecture Hiérarchique:

```
🌳 Behavior Tree: FlyingDroneRedBehavior
│
└─ ⚙️ ROOT (Selector)
    │
    ├─ ⚙️ SEQUENCE: "1. Emergency Evasion" ⭐ PRIORITÉ 1
    │   ├─ ✓ CheckLowHealth
    │   │   └─ healthThreshold = 30.0
    │   │
    │   ├─ ⚡ UnlockCurrentTarget
    │   │
    │   ├─ ⚡ EvasiveManeuver "Dodge!"
    │   │   ├─ target = [Shared: target]
    │   │   ├─ evasionRadius = 5.0
    │   │   ├─ evasionSpeed = 10.0
    │   │   └─ maneuverDuration = 1.5
    │   │
    │   └─ ⚡ TacticalRetreat "Flee"
    │       ├─ retreatDistance = 25.0
    │       └─ target = [Shared: target]
    │
    ├─ ⚙️ PARALLEL: "2. Combat + Movement" ⭐ PRIORITÉ 2
    │   │  (Success: Immediate, Failure: Immediate)
    │   │
    │   ├─ Branch 1: ⚙️ SEQUENCE "Aggressive Targeting"
    │   │   │
    │   │   ├─ ⚙️ SELECTOR: "Pick Target"
    │   │   │   │
    │   │   │   ├─ ⚡ SelectWeakestEnemy "Hunt Turrets"
    │   │   │   │   ├─ enemyType = "Turret"
    │   │   │   │   ├─ target = [Shared: target]
    │   │   │   │   ├─ minRadius = 0.0
    │   │   │   │   └─ maxRadius = 100.0
    │   │   │   │
    │   │   │   └─ ⚡ SelectWeakestEnemy "Hunt Drones"
    │   │   │       ├─ enemyType = "Drone"
    │   │   │       └─ target = [Shared: target]
    │   │   │
    │   │   └─ 🔄 Repeat Until Failure
    │   │
    │   ├─ Branch 2: ⚙️ SEQUENCE "Evasive Movement"
    │   │   │
    │   │   ├─ ⚡ CircleStrafeTarget "Strafe!"
    │   │   │   ├─ target = [Shared: target]
    │   │   │   ├─ strafeDistance = 15.0
    │   │   │   ├─ strafeSpeed = 45.0
    │   │   │   └─ clockwise = true
    │   │   │
    │   │   ├─ ⚡ MyFlySeek "Fly to Target"
    │   │   │   ├─ m_Target = [Shared: target]
    │   │   │   └─ m_TranslationMaxSpeed = 10.0
    │   │   │
    │   │   └─ 🔄 Repeat
    │   │
    │   └─ Branch 3: ⚙️ SEQUENCE "Continuous Fire"
    │       │
    │       ├─ ⚡ FlyingDroneShoot "Shoot!"
    │       │   └─ m_ShootingPeriod = 0.5
    │       │
    │       └─ 🔄 Repeat
    │
    └─ ⚡ Wait "Idle"
        └─ duration = 1.0
```

### Variables Partagées:

```
Variable Name       | Type      | Is Shared | Value
--------------------|-----------|-----------|-------
target              | Transform | YES       | null
minRadius           | Float     | YES       | 0.0
maxRadius           | Float     | YES       | 100.0
strafeDistance      | Float     | NO        | 15.0
strafeSpeed         | Float     | NO        | 45.0
evasionSpeed        | Float     | NO        | 10.0
```

---

## 🏰 TOURELLE ROUGE - Configuration Optimisée

### Architecture Hiérarchique:

```
🌳 Behavior Tree: TurretRedBehaviorAdvanced
│
└─ ⚙️ ROOT (Selector)
    │
    ├─ ⚙️ SEQUENCE: "1. Focused Fire" ⭐ PRIORITÉ 1
    │   │
    │   ├─ ⚙️ SELECTOR: "Priority Targeting"
    │   │   │
    │   │   ├─ ⚡ SelectWeakestEnemy "Kill Turrets"
    │   │   │   ├─ enemyType = "Turret"
    │   │   │   ├─ target = [Shared: target]
    │   │   │   ├─ minRadius = 0.0
    │   │   │   └─ maxRadius = 100.0
    │   │   │
    │   │   └─ ⚡ SelectWeakestEnemy "Kill Drones"
    │   │       ├─ enemyType = "Drone"
    │   │       ├─ target = [Shared: target]
    │   │       ├─ minRadius = 0.0
    │   │       └─ maxRadius = 100.0
    │   │
    │   ├─ ✓ CheckTargetInRange "Validate Range"
    │   │   ├─ target = [Shared: target]
    │   │   ├─ minRange = 0.0
    │   │   └─ maxRange = 100.0
    │   │
    │   ├─ ⚡ TurretSeekTarget "Aim"
    │   │   └─ target = [Shared: target]
    │   │
    │   ├─ ⚡ TurretShoot "Fire Rocket!"
    │   │   └─ target = [Shared: target]
    │   │
    │   ├─ ⚡ WaitRandom "Reload"
    │   │   ├─ minWaitTime = 1.5
    │   │   └─ maxWaitTime = 2.5
    │   │
    │   └─ 🔄 Repeat Until Failure
    │
    └─ ⚡ Wait "Standby"
        └─ duration = 2.0
```

### Variables Partagées:

```
Variable Name  | Type      | Is Shared | Value
---------------|-----------|-----------|-------
target         | Transform | YES       | null
minRadius      | Float     | YES       | 0.0
maxRadius      | Float     | YES       | 100.0
```

---

## 🎯 FLOW DÉCISIONNEL VISUEL

### Drone Rouge - Arbre de décision:

```
                    ┌─────────────┐
                    │   START     │
                    └──────┬──────┘
                           │
                    ┌──────▼──────┐
                    │ HP < 30 ?   │
                    └──┬───────┬──┘
                   Oui │       │ Non
              ┌────────▼──┐    │
              │ REPLI     │    │
              │ TACTIQUE  │    │
              └───────────┘    │
                           ┌───▼────────┐
                           │Cible actuelle│
                           │faible ?      │
                           └──┬───────┬──┘
                          Oui │       │ Non
                  ┌───────────▼──┐    │
                  │ MAINTENIR    │    │
                  │ FOCUS        │    │
                  └──────────────┘    │
                                  ┌───▼────────┐
                                  │Chercher     │
                                  │tourelle     │
                                  │faible       │
                                  └──┬───────┬──┘
                                 Oui │       │ Non
                         ┌───────────▼──┐    │
                         │ ATTAQUE      │    │
                         │ TOURELLE     │    │
                         └──────────────┘    │
                                         ┌───▼────────┐
                                         │Chercher     │
                                         │drone faible │
                                         └──┬──────────┘
                                            │
                                    ┌───────▼─────┐
                                    │ KITING +    │
                                    │ ATTAQUE     │
                                    └─────────────┘
```

---

## ⚙️ CONFIGURATION DANS UNITY

### Étape 1: Créer le Behavior Tree Asset

1. Clic droit dans `Assets/Scripts/MyBehaviorTrees/`
2. `Create → Behavior Designer → Behavior Tree`
3. Nommer: `DroneRedBehaviorAdvanced`

### Étape 2: Ouvrir Behavior Designer

1. `Window → Behavior Designer → Editor`
2. `File → Load External Behavior Tree`
3. Sélectionner `DroneRedBehaviorAdvanced`

### Étape 3: Construire l'arbre

**Pour chaque node:**

1. **Clic droit dans l'éditeur**
2. **Choisir le type:**
   - `Add Task → Composites → Selector/Sequence/Parallel`
   - `Add Task → MyTasks → [Nom de la tâche]`

3. **Configurer les paramètres:**
   - Clic sur le node
   - Inspector à droite affiche les paramètres
   - Remplir les valeurs

4. **Connecter les nodes:**
   - Glisser depuis le connecteur du parent vers l'enfant

### Étape 4: Configuration d'un node exemple

**Exemple: SelectWeakestEnemy**

```
Inspector Panel:
┌──────────────────────────────────┐
│ SelectWeakestEnemy              │
├──────────────────────────────────┤
│ Enemy Type: [Dropdown]          │
│   • Drone                        │
│   • Turret                ✓      │
│                                  │
│ Target: [Shared Transform]      │
│   Variable: target               │
│                                  │
│ Min Radius: [Float]             │
│   Value: 0.0                     │
│                                  │
│ Max Radius: [Float]             │
│   Value: 100.0                   │
└──────────────────────────────────┘
```

---

## 🔍 DEBUGGING EN TEMPS RÉEL

### Visualisation pendant Play Mode:

Quand vous lancez le jeu, Behavior Designer affiche:

```
🟢 = Success (vert) - Le node a réussi
🔴 = Failure (rouge) - Le node a échoué
🔵 = Running (bleu) - Le node est en cours
⚪ = Inactive (gris) - Pas encore exécuté

Exemple pendant combat:
┌─ ROOT (Selector) 🟢
    ├─ Survival Mode (Sequence) 🔴
    │   └─ CheckLowHealth ❌ (HP=75, seuil=30)
    │
    ├─ Combat Intelligent (Sequence) 🔵 ← EN COURS
    │   ├─ Target Selection (Selector) 🟢
    │   │   └─ SelectWeakestEnemy ✓ (Tourelle trouvée)
    │   ├─ KeepDistance 🔵 ← ACTIF
    │   ├─ MySeek 🔵 ← ACTIF
    │   └─ DroneShoot ⚪ ← Prochain
    │
    └─ Idle ⚪
```

---

## 📊 METRICS DE PERFORMANCE

### Logs Unity Console - Ce que vous verrez:

```
Frame 120:
[AI-STRATEGY] DroneRed_01 maintaining focus on weakened target TurretGreen_03 (35 HP)
[AI-KITE] DroneRed_01 repositioning (too close): current=12.3, optimal=15.0
[AI-WEAK] DroneRed_02 targeting weakest Turret: TurretGreen_03 with 35 HP

Frame 145:
[AI-FOCUS] Target TurretGreen_03 is weakened (22 HP) - maintaining focus!
[AI-PRIORITY] DroneRed_02 selected priority Turret: TurretGreen_03
[AI-STRATEGY] DroneRed_01 maintaining focus on weakened target TurretGreen_03 (22 HP)
^ Focus fire coordonné: 2 unités sur même cible!

Frame 178:
[AI-RETREAT] DroneRed_01 is low on health (28/30) - retreating!
[AI-UNLOCK] DroneRed_01 unlocked its target
[AI-RETREAT] DroneRed_01 retreating to (45.2, 0, 78.9)
^ Repli tactique activé

Frame 203:
[AI-STEAL] DroneRed_03 stole weakened target TurretGreen_02 (18 HP) from DroneRed_04
^ Vol de cible pour finir l'ennemi
```

---

## 🎓 TIPS DE CONFIGURATION

### ✅ Bonnes Pratiques:

1. **Nommer les nodes** - Double-clic sur node → Rename
   - Exemple: "Check Health" → "1.1 Survival Check"

2. **Utiliser les couleurs** - Clic droit → Change Color
   - Rouge = Survie
   - Jaune = Combat
   - Vert = Idle

3. **Commenter** - Ajouter des "Comment" nodes pour documentation

4. **Tester progressivement**:
   - Commencer avec un simple Sequence
   - Ajouter complexité graduellement
   - Tester après chaque ajout

5. **Variables partagées**:
   - Utilisez "Shared" pour variables communes
   - Private pour paramètres spécifiques

### ❌ Erreurs Courantes:

1. **Oublier de connecter les nodes** → Behavior ne s'exécute pas
2. **Mauvais type de parent** → Sequence au lieu de Selector
3. **Variables non assignées** → NullReferenceException
4. **Boucles infinies** → Repeat sans condition de sortie
5. **Conditions contradictoires** → Node jamais Success

---

## 🏆 CHECKLIST DE VALIDATION

Avant de tester:

- [ ] Tous les nodes sont connectés (pas d'orphelins)
- [ ] Variables partagées définies
- [ ] Paramètres remplis (pas de NaN ou null)
- [ ] Structure logique (priorités correctes)
- [ ] External Behavior assigné au prefab
- [ ] Console Unity sans erreurs
- [ ] Behavior Designer sans warnings

---

**Vous êtes prêt! Construisez ces arbres et regardez l'armée rouge dominer! 🔴🎯🏆**
