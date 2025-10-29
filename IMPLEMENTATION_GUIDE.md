# 🎯 GUIDE D'IMPLÉMENTATION UNITY - IA ARMÉE ROUGE

## 🚀 ÉTAPES D'INSTALLATION

### Phase 1: Compilation des nouveaux scripts

1. **Ouvrir Unity**
   - Lancer le projet `JeuxIA`
   - Attendre que Unity compile automatiquement les nouveaux scripts
   - Vérifier la console pour des erreurs de compilation

2. **Vérifier les nouveaux comportements**
   - Dans Behavior Designer: `Window → Behavior Designer → Editor`
   - Vérifier que les nouvelles tâches apparaissent dans la catégorie `MyTasks`

---

## 🎨 CRÉATION DES BEHAVIOR TREES

### 🔴 Drone Rouge (Sol) - Configuration complète

#### Étape 1: Dupliquer le behavior tree existant
1. Dans `Assets/Scripts/MyBehaviorTrees/`
2. Dupliquer `DroneRedBehavior.asset` → `DroneRedBehaviorAdvanced.asset`
3. Double-cliquer pour ouvrir dans Behavior Designer

#### Étape 2: Créer l'arbre de comportement

**Structure recommandée:**

```
ROOT (Selector)
├── SEQUENCE: "Survival Mode" 
│   ├── CheckLowHealth (healthThreshold: 30)
│   ├── UnlockCurrentTarget
│   └── TacticalRetreat (retreatDistance: 20, target)
│
├── SEQUENCE: "Combat Intelligent"
│   ├── SELECTOR: "Target Selection"
│   │   ├── SEQUENCE: "Keep Weak Target"
│   │   │   ├── CheckTargetHealth (healthThreshold: 40, target)
│   │   │   └── Success
│   │   ├── SelectWeakestEnemy (enemyType: "Turret", minRadius: 0, maxRadius: 100, target)
│   │   └── SelectWeakestEnemy (enemyType: "Drone", minRadius: 0, maxRadius: 100, target)
│   │
│   ├── KeepDistance (target, optimalDistance: 15, distanceTolerance: 3)
│   ├── CheckTargetInRange (target, minRange: 0, maxRange: 25)
│   ├── MySeek (target, arriveDistance: 2)
│   ├── DroneShoot
│   └── WaitRandom (minWaitTime: 0.3, maxWaitTime: 0.8)
│
└── SEQUENCE: "Idle"
    └── WaitRandom (minWaitTime: 1, maxWaitTime: 3)
```

#### Étape 3: Configuration des variables partagées

Dans l'onglet "Variables" du Behavior Designer:

| Nom | Type | Valeur initiale | Visibilité |
|-----|------|-----------------|------------|
| `target` | SharedTransform | null | Shared |
| `minRadius` | SharedFloat | 0 | Shared |
| `maxRadius` | SharedFloat | 100 | Shared |
| `healthThreshold` | SharedFloat | 30 | Private |
| `optimalDistance` | SharedFloat | 15 | Private |

#### Étape 4: Assigner au prefab
1. Sélectionner `Assets/Prefabs/DroneRed.prefab`
2. Dans `Behavior Tree` component: 
   - External Behavior → `DroneRedBehaviorAdvanced`
3. Sauvegarder le prefab

---

### 🚁 Flying Drone Rouge - Configuration avancée

**Note importante:** Les verts n'ont pas de FlyingDrone, alors créons-en un pour les rouges!

#### Étape 1: Créer le prefab FlyingDroneRed

1. **Dupliquer le prefab vert:**
   - `Assets/Prefabs/FlyingDroneGreen.prefab` → `FlyingDroneRed.prefab`

2. **Modifier le tag:**
   - Sélectionner `FlyingDroneRed.prefab`
   - Tag: `Red`

3. **Modifier les matériaux:**
   - Changer la couleur en rouge pour le différencier visuellement

#### Étape 2: Créer le behavior tree

1. Créer un nouveau asset: 
   - `Assets/Scripts/MyBehaviorTrees/FlyingDroneRedBehavior.asset`

**Structure recommandée:**

```
ROOT (Selector)
├── SEQUENCE: "Emergency Evasion"
│   ├── CheckLowHealth (healthThreshold: 30)
│   ├── UnlockCurrentTarget
│   ├── EvasiveManeuver (target, evasionRadius: 5, evasionSpeed: 10, maneuverDuration: 1.5)
│   └── TacticalRetreat (retreatDistance: 25, target)
│
├── PARALLEL: "Combat + Movement"
│   ├── SEQUENCE: "Aggressive Targeting"
│   │   ├── SELECTOR: "Pick Target"
│   │   │   ├── SelectWeakestEnemy (enemyType: "Turret", target)
│   │   │   └── SelectWeakestEnemy (enemyType: "Drone", target)
│   │   └── Repeat Until Failure
│   │
│   ├── SEQUENCE: "Evasive Combat"
│   │   ├── CircleStrafeTarget (target, strafeDistance: 15, strafeSpeed: 45, clockwise: true)
│   │   ├── MyFlySeek (m_Target: target, m_TranslationMaxSpeed: 10)
│   │   └── Repeat
│   │
│   └── SEQUENCE: "Continuous Fire"
│       ├── FlyingDroneShoot (m_ShootingPeriod: 0.5)
│       └── Repeat
│
└── Wait (1 second)
```

#### Étape 3: Assigner au prefab
- `FlyingDroneRed.prefab` → External Behavior: `FlyingDroneRedBehavior`

---

### 🏰 Tourelle Rouge - Configuration optimisée

#### Étape 1: Dupliquer
- `TurretRedBehavior.asset` → `TurretRedBehaviorAdvanced.asset`

#### Étape 2: Structure optimisée

```
ROOT (Selector)
├── SEQUENCE: "Focused Fire"
│   ├── SELECTOR: "Priority Targeting"
│   │   ├── SelectWeakestEnemy (enemyType: "Turret", target, minRadius: 0, maxRadius: 100)
│   │   └── SelectWeakestEnemy (enemyType: "Drone", target, minRadius: 0, maxRadius: 100)
│   │
│   ├── CheckTargetInRange (target, minRange: 0, maxRange: 100)
│   ├── TurretSeekTarget (target)
│   ├── TurretShoot (target)
│   ├── WaitRandom (minWaitTime: 1.5, maxWaitTime: 2.5)
│   └── Repeat Until Failure
│
└── Wait (2 seconds)
```

#### Étape 3: Assigner
- `TurretRed.prefab` → External Behavior: `TurretRedBehaviorAdvanced`

---

## 🎮 CONFIGURATION DE LA SCÈNE

### Spawner les Flying Drones Rouges

Si vous utilisez un spawner/manager pour instancier les unités:

1. **Trouver l'Army Manager Red dans la scène**
   - `Hierarchy → ArmyManagerRed`

2. **Ajouter des FlyingDroneRed au spawn**
   - Dans le script de spawn ou directement dans la scène
   - Ajouter 2-3 `FlyingDroneRed` pour équilibrer

3. **Position initiale**
   - Placer à une hauteur appropriée (Y = 10-15)
   - Disperser sur la zone de spawn rouge

---

## ⚙️ PARAMÈTRES RECOMMANDÉS

### Configuration initiale (Conservative)

| Unité | Health Threshold | Optimal Distance | Special |
|-------|------------------|------------------|---------|
| **Drone Rouge** | 30 HP | 15m | Kiting actif |
| **Flying Drone Rouge** | 30 HP | 15m | Strafe 45°/s |
| **Turret Rouge** | N/A | N/A | Focus fire |

### Configuration agressive (Si trop facile)

| Unité | Health Threshold | Optimal Distance | Special |
|-------|------------------|------------------|---------|
| **Drone Rouge** | 25 HP | 18m | Kiting + agressif |
| **Flying Drone Rouge** | 25 HP | 12m | Strafe 60°/s |
| **Turret Rouge** | N/A | N/A | Cadence réduite |

---

## 🧪 TESTS ET VALIDATION

### Test 1: Compilation
```
1. Unity Editor → Window → Console
2. Vérifier aucune erreur rouge
3. Warnings jaunes acceptables
```

### Test 2: Behavior Trees
```
1. Ouvrir Behavior Designer
2. Charger chaque behavior tree
3. Vérifier connexions
4. Run → Tester en Play Mode
```

### Test 3: Combat
```
1. Lancer la scène RedVersusGreenBattle
2. Observer les logs dans Console
3. Vérifier les comportements:
   ✓ Repli quand vie basse
   ✓ Focus fire sur cibles faibles
   ✓ Kiting visible
   ✓ Strafing des drones volants
```

### Test 4: Logs de debug

Rechercher dans la console:
- `[AI-STRATEGY]` - Décisions stratégiques
- `[AI-FOCUS]` - Focus fire
- `[AI-WEAK]` - Sélection cibles faibles
- `[AI-KITE]` - Repositionnement kiting
- `[AI-RETREAT]` - Repli tactique
- `[AI-STEAL]` - Vol de cible
- `[AI-EVADE]` - Manœuvres évasives

---

## 🐛 TROUBLESHOOTING

### Problème: Scripts ne compilent pas

**Solution:**
```
1. Vérifier que Behavior Designer est bien installé
2. Assets → Reimport All
3. Edit → Project Settings → Player → Scripting Define Symbols
   Vérifier: BEHAVIOR_DESIGNER
```

### Problème: Les tâches n'apparaissent pas

**Solution:**
```
1. Window → Behavior Designer → Editor
2. Tools → Check for Errors
3. Si problème: Reimport scripts
```

### Problème: Les drones ne bougent pas

**Solution:**
```
1. Vérifier NavMesh présent dans la scène
2. Window → AI → Navigation
3. Bake le NavMesh si nécessaire
4. Vérifier que les drones ont NavMeshAgent component
```

### Problème: Les rouges sont trop forts

**Ajustements:**
```
1. Augmenter healthThreshold → 40 (repli plus tôt)
2. Réduire stealHealthThreshold → 20 (moins agressif)
3. Augmenter optimalDistance → 20 (plus défensif)
4. Réduire focusFireThreshold → 30 (moins de coordination)
```

### Problème: Les rouges perdent encore

**Ajustements:**
```
1. Réduire healthThreshold → 25 (repli plus tard)
2. Augmenter stealHealthThreshold → 40 (plus agressif)
3. Réduire optimalDistance → 12 (plus offensif)
4. Augmenter strafeSpeed → 60°/s (plus évasif)
5. AJOUTER plus de FlyingDroneRed (2-3 unités)
```

---

## 📊 MÉTRIQUES À OBSERVER

### Pendant le test:

1. **Taux de survie rouge**
   - Nombre d'unités rouges survivantes
   - Objectif: 2-4 unités

2. **Temps de victoire**
   - Chronomètre dans l'interface
   - Objectif: < 120 secondes

3. **Comportements tactiques**
   - Nombre de replis réussis (logs `[AI-RETREAT]`)
   - Nombre de focus fire (logs `[AI-FOCUS]`)
   - Nombre de vols de cible (logs `[AI-STEAL]`)

4. **HP résiduel**
   - Santé moyenne des survivants rouges
   - Objectif: > 50 HP moyen

---

## 🎓 AMÉLIORATIONS OPTIONNELLES

### Si vous voulez aller plus loin:

1. **Ajouter des formations**
   ```csharp
   // Créer FormationManager.cs
   // Coordonner positions relatives des unités
   ```

2. **Prédiction de trajectoire**
   ```csharp
   // Dans les scripts de shoot
   // Calculer lead target pour projectiles
   ```

3. **Machine Learning** (Avancé)
   ```csharp
   // Utiliser ML-Agents de Unity
   // Entraîner les comportements
   ```

4. **Système de communication**
   ```csharp
   // EventSystem pour partager info
   // "Ennemi affaibli en X,Y,Z"
   ```

---

## ✅ CHECKLIST FINALE

Avant de tester la bataille complète:

- [ ] Tous les scripts compilent sans erreur
- [ ] Les 3 behavior trees sont créés et configurés
- [ ] Les prefabs référencent les bons behavior trees
- [ ] Le NavMesh est baked
- [ ] Des FlyingDroneRed sont ajoutés à la scène
- [ ] Les variables partagées sont configurées
- [ ] La console affiche les logs de debug
- [ ] Un test rapide confirme les comportements de base

---

## 🏆 RÉSULTAT ATTENDU

Après implémentation complète:

**Avant:**
```
🟢 Armée Verte: 50% victoires
🔴 Armée Rouge: 50% victoires
```

**Après:**
```
🟢 Armée Verte: 20-30% victoires
🔴 Armée Rouge: 70-80% victoires
```

**Supériorité stratégique confirmée! 🔴🎯**

---

## 📞 SUPPORT

En cas de problème:

1. **Vérifier les logs Unity** - Beaucoup d'info dans `[AI-*]` logs
2. **Behavior Designer debugger** - Visualiser l'arbre en temps réel
3. **Tester unité par unité** - Isoler les problèmes
4. **Ajuster progressivement** - Ne pas tout changer d'un coup

**Bon développement! 🚀**
