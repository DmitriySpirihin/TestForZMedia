<?xml version="1.0" encoding="UTF-8"?>
<Readme>
  <Project>
    <Name>TestForZMedia</Name>
    <Title>Tiny Army Battle Simulator</Title>
  </Project>

  <Overview>
    <Description>
      Данный проект реализует симулятор боя для мобильных платформ согласно требованиям тестового задания. 
      Две армии по 20 юнитов вступают в ближний бой с рандомизированными характеристиками. 
      Проект демонстрирует чистую архитектуру, реактивное программирование и data-driven дизайн, 
      подходящий для мобильного развертывания.
    </Description>
    <TimeSpent>
      Приблизительно 8-12 часов целенаправленной разработки, включая проектирование архитектуры, 
      реализацию, тестирование и документирование.
    </TimeSpent>
  </Overview>

  <Architecture>
    <Principles>
      <Principle>
        <Name>Разделение ответственности</Name>
        <Description>
          Логика (UnitData), представление (MonoBehaviour компоненты) и конфигурация (ScriptableObject) 
          строго разделены
        </Description>
      </Principle>
      <Principle>
        <Name>Внедрение зависимостей</Name>
        <Description>Zenject управляет всеми межкомпонентными зависимостями</Description>
      </Principle>
      <Principle>
        <Name>Реактивное управление состоянием</Name>
        <Description>UniRx обрабатывает распространение состояний без опроса (polling)</Description>
      </Principle>
      <Principle>
        <Name>Data-driven дизайн</Name>
        <Description>Все параметры баланса вынесены в ассеты ScriptableObject</Description>
      </Principle>
    </Principles>

    <Components>
      <Component>
        <Name>BattleManager</Name>
        <Role>Оркестратор</Role>
        <Responsibilities>
          <Item>UnitData[] _armyA, _armyB — логическое состояние</Item>
          <Item>Dictionary&lt;(ArmyType, int), UnitView&gt; _unitViews — поиск представлений</Item>
          <Item>Dictionary&lt;ArmyType, FormationConfigSO&gt; _armyFormations — конфиги формаций</Item>
        </Responsibilities>
      </Component>

      <Component>
        <Name>UnitView</Name>
        <Role>Корень композиции юнита</Role>
        <SubComponents>
          <Item>UnitHealth : IHealth — управление здоровьем с реактивными свойствами</Item>
          <Item>UnitMove : IMove — движение с отслеживанием цели</Item>
          <Item>UnitAttack : IAttack — логика атаки с задержкой ATKSPD</Item>
          <Item>ApplyVisualConfig() — применение материалов/масштаба</Item>
        </SubComponents>
      </Component>

      <Component>
        <Name>UnitCreator</Name>
        <Role>Генерация данных</Role>
        <Method>
          <Name>CreateUnit()</Name>
          <Description>Рандомизирует конфиги, считает статы, возвращает UnitData</Description>
        </Method>
      </Component>

      <Component>
        <Name>UnitFactory</Name>
        <Role>Создание представлений</Role>
        <Method>
          <Name>Create()</Name>
          <Description>Инстанцирование префабов через Zenject с инъекцией зависимостей</Description>
        </Method>
      </Component>

      <Component>
        <Name>ArmySpawner</Name>
        <Role>Композиция армии</Role>
        <Method>
          <Name>SpawnArmy()</Name>
          <Description>Координация Creator + Factory + оффсетов формации</Description>
        </Method>
      </Component>
    </Components>
  </Architecture>

  <DesignDecisions>
    <Decision>
      <Number>1</Number>
      <Name>UnitData как value-type структура</Name>
      <Code>
        <![CDATA[
public struct UnitData
{
    public int Id;
    public ArmyType Army;
    public float CurrentHP, MaxHP, ATK, SPEED, ATKSPD;
    public Vector2 FormationOffset;
    public ShapeConfigSO ShapeConfig;
    public ColorConfigSO ColorConfig;
    public ScaleConfigSO SizeConfig;
    
    public bool IsAlive => CurrentHP > 0f;
}
        ]]>
      </Code>
      <Rationale>
        <Item>Структуры копируются по значению, что исключает баги, связанные с ссылками, в логике боя</Item>
        <Item>Нет аллокаций в куче во время цикла боя (критично для мобильного GC)</Item>
        <Item>Четкий контракт между слоем логики (BattleManager) и слоем представления (UnitView)</Item>
        <Item>Ссылки на конфиги — легковесные указатели, не тяжелые копии</Item>
      </Rationale>
    </Decision>

    <Decision>
      <Number>2</Number>
      <Name>ReactiveProperty для управления состоянием</Name>
      <Code>
        <![CDATA[
public class UnitHealth : MonoBehaviour, IHealth
{
    private readonly ReactiveProperty<float> _currentHealth = new();
    private readonly ReactiveProperty<HealthState> _state = new(HealthState.Alive);
    
    public IReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
    public IReadOnlyReactiveProperty<HealthState> State => _state;
    
    public void Init(UnitData data)
    {
        _currentHealth.Value = data.MaxHP;
        _currentHealth.Where(hp => hp <= 0f).Take(1)
            .Subscribe(_ => OnDeath()).AddTo(_disposables);
    }
}
        ]]>
      </Code>
      <Rationale>
        <Item>UI и игровая логика подписываются на изменения состояния без опроса</Item>
        <Item>IReadOnlyReactiveProperty&lt;T&gt; инкапсулирует доступ на запись, предотвращая внешнюю мутацию</Item>
        <Item>CompositeDisposable обеспечивает автоматическую очистку при уничтожении компонента</Item>
        <Item>.Take(1) гарантирует, что логика смерти выполнится ровно один раз</Item>
      </Rationale>
    </Decision>

    <Decision>
      <Number>3</Number>
      <Name>Поиск цели через BattleManager</Name>
      <Code>
        <![CDATA[
public UnitView FindNearestEnemy(ArmyType myArmy, Vector3 position)
{
    var enemyArray = (myArmy == ArmyType.ArmyA) ? _armyB : _armyA;
    var enemyArmyType = (myArmy == ArmyType.ArmyA) ? ArmyType.ArmyB : ArmyType.ArmyA;
    
    UnitView nearest = null;
    float minSqrDist = float.MaxValue;
    
    foreach (var enemyData in enemyArray)
    {
        if (!enemyData.IsAlive) continue;
        
        var key = (enemyArmyType, enemyData.Id);
        if (!_unitViews.TryGetValue(key, out var enemyView) || enemyView == null)
            continue;
        
        float sqrDist = Vector3.SqrMagnitude(position - enemyView.transform.position);
        
        if (sqrDist < minSqrDist)
        {
            minSqrDist = sqrDist;
            nearest = enemyView;
        }
    }
    return nearest;
}
        ]]>
      </Code>
      <Rationale>
        <Item>Централизованная логика поиска цели позволяет добавлять улучшения без модификации отдельных юнитов</Item>
        <Item>SqrMagnitude избегает дорогостоящих вычислений квадратного корня (40 юнитов × 60 FPS)</Item>
        <Item>Поиск по словарю с tuple-ключом обеспечивает O(1) доступ и уникальность для обеих армий</Item>
      </Rationale>
    </Decision>

    <Decision>
      <Number>4</Number>
      <Name>Система формаций как дополнительная функция</Name>
      <Code>
        <![CDATA[
public void ChangeFormation()
{
    var formation = _battleManager.GetFormation(_data.Army);
    Vector2 offset = Vector2.zero;
    
    if (formation?.FORMATION_OFFSETS != null && _data.Id < formation.FORMATION_OFFSETS.Length)
        offset = formation.FORMATION_OFFSETS[_data.Id];
    
    transform.position = _armyCenter + new Vector3(offset.x, 0f, offset.y);
}
        ]]>
      </Code>
      <Rationale>
        <Item>Оффсеты, хранящиеся в ScriptableObject, позволяют создавать новые формации без изменения кода</Item>
        <Item>Параметр armyCenter позволяет динамически перемещать формации (например, для предпросмотра в меню)</Item>
        <Item>Оффсеты кратны 2 единицам для предотвращения начального перекрытия коллайдеров</Item>
      </Rationale>
    </Decision>

    <Decision>
      <Number>5</Number>
      <Name>MaterialPropertyBlock для визуальной кастомизации</Name>
      <Code>
        <![CDATA[
public void ApplyVisualConfig()
{
    var renderer = GetComponent<Renderer>();
    var mpb = new MaterialPropertyBlock();
    renderer.GetPropertyBlock(mpb);
    mpb.SetColor("_BaseColor", _data.ColorConfig.COLOR);
    renderer.SetPropertyBlock(mpb);
    
    transform.localScale = Vector3.one * _data.ScaleConfig.SCALE;
}
        ]]>
      </Code>
      <Rationale>
        <Item>Изменение цвета материала через MaterialPropertyBlock сохраняет GPU батчинг</Item>
        <Item>Критично для мобильной производительности с 40+ юнитами на экране</Item>
        <Item>Изменение масштаба использует простое умножение, избегая дорогостоящих изменений иерархии</Item>
      </Rationale>
    </Decision>
  </DesignDecisions>

  <CodeStructure>
    <Folder path="Assets/Scripts/Core">
      <File name="BattleManager.cs">Оркестрация боя, условия победы</File>
      <File name="UnitData.cs">Value-type контракт данных</File>
      <File name="GameEnums.cs">Общие определения enum</File>
    </Folder>
    <Folder path="Assets/Scripts/Units">
      <File name="UnitView.cs">Корень композиции компонентов</File>
      <File name="UnitHealth.cs">Реализация IHealth</File>
      <File name="UnitMove.cs">Реализация IMove</File>
      <File name="UnitAttack.cs">Реализация IAttack</File>
    </Folder>
    <Folder path="Assets/Scripts/Systems">
      <File name="UnitCreator.cs">Генерация данных с рандомизацией</File>
      <File name="UnitFactory.cs">Инстанцирование префабов через Zenject</File>
      <File name="ArmySpawner.cs">Композиция армии с формациями</File>
    </Folder>
    <Folder path="Assets/Scripts/UI">
      <File name="UiPresenter.cs">Управление UI на основе GameState</File>
    </Folder>
    <Folder path="Assets/Scripts/Config">
      <File name="GameConfigSO.cs">Базовые статы (HP, ATK, SPEED, ATKSPD)</File>
      <File name="ShapeConfigSO.cs">Ссылка на префаб + модификаторы формы</File>
      <File name="ColorConfigSO.cs">Значение цвета + модификаторы цвета</File>
      <File name="ScaleConfigSO.cs">Значение масштаба + модификаторы размера</File>
      <File name="FormationConfigSO.cs">Массив Vector2 оффсетов формации</File>
    </Folder>
    <Folder path="Assets/Installers">
      <File name="CoreInstaller.cs">Конфигурация биндов Zenject</File>
    </Folder>
    <Folder path="Assets/Prefabs">
      <File name="UnitCube.prefab"/>
      <File name="UnitSphere.prefab"/>
      <File name="UI/..."/>
    </Folder>
  </CodeStructure>

  <AdditionalFeature>
    <Name>Рандомайзер формаций</Name>
    <Description>
      Проект включает систему тактических формаций как требуемую дополнительную функцию:
    </Description>
    <Features>
      <Item>Формации определяются как ассеты FormationConfigSO, содержащие массив из 20 Vector2 оффсетов</Item>
      <Item>Оффсеты применяются при спавне: spawnPosition = armyCenter + offset</Item>
      <Item>Метод ChangeFormation() позволяет динамически перепозиционировать без респавна</Item>
      <Item>Формации можно просматривать в главном меню перед началом боя</Item>
    </Features>
    <Extensibility>
      Добавление новой формации требует только создания нового ассета ScriptableObject, без изменений кода.
    </Extensibility>
  </AdditionalFeature>

  <HowToRun>
    <Step>Откройте проект в Unity 2021.3 LTS или новее</Step>
    <Step>Убедитесь, что все конфиги ScriptableObject назначены в инспекторе CoreInstaller</Step>
    <Step>Откройте BattleScene и нажмите Play</Step>
    <Step>
      <Description>В главном меню:</Description>
      <SubStep>Используйте кнопки "Formation A/B" для предпросмотра различных формаций</SubStep>
      <SubStep>Нажмите "Start Battle" для начала боя</SubStep>
    </Step>
    <Step>Бой заканчивается автоматически, когда одна армия уничтожена; нажмите "Restart" для возврата в меню</Step>
  </HowToRun>

  <PlatformAndOptimization>
    <TargetPlatform>Мобильные устройства Android/iOS</TargetPlatform>
    <TestedResolution>1920×1080 (ландшафтная ориентация)</TestedResolution>
    <Optimizations>
      <Item>
        <Name>Vector3.SqrMagnitude</Name>
        <Benefit>Для сравнения дистанций (избегает sqrt)</Benefit>
      </Item>
      <Item>
        <Name>MaterialPropertyBlock</Name>
        <Benefit>Для изменения цвета (сохраняет батчинг)</Benefit>
      </Item>
      <Item>
        <Name>UnitData на основе структур</Name>
        <Benefit>Нет аллокаций в куче в цикле боя</Benefit>
      </Item>
      <Item>
        <Name>UniRx CompositeDisposable</Name>
        <Benefit>Для автоматической очистки подписок</Benefit>
      </Item>
      <Item>
        <Name>Object pooling не реализован</Name>
        <Benefit>40 юнитов × один бой не требуют этого</Benefit>
      </Item>
    </Optimizations>
  </PlatformAndOptimization>

  <FutureImprovements>
    <Description>
      Хотя текущая реализация удовлетворяет всем требованиям тестового задания, 
      архитектура поддерживает следующие расширения без рефакторинга:
    </Description>
    <Improvement>
      <Name>Object Pooling</Name>
      <Description>Добавить сервис UnitPool для сценариев с частым спавном/деспавном</Description>
    </Improvement>
    <Improvement>
      <Name>Продвинутый таргетинг</Name>
      <Description>Реализовать правила приоритета (низкое HP, высокий ATK) в BattleManager.FindNearestEnemy</Description>
    </Improvement>
    <Improvement>
      <Name>Визуальные эффекты</Name>
      <Description>Подписаться на UnitHealth.State для триггеров VFX попаданий/смерти</Description>
    </Improvement>
    <Improvement>
      <Name>Сетевая синхронизация</Name>
      <Description>Извлечь логику боя в чистые C# классы для серверно-авторитативного мультиплеера</Description>
    </Improvement>
    <Improvement>
      <Name>Система сохранений</Name>
      <Description>Сериализовать массивы UnitData для повторов боя или функций прогрессии</Description>
    </Improvement>
  </FutureImprovements>

  <Conclusion>
    Данный проект демонстрирует готовую к продакшену архитектуру для мобильного симулятора боя. 
    Комбинация Zenject для управления зависимостями, UniRx для реактивного состояния и ScriptableObject 
    для data-driven дизайна создает поддерживаемую, тестируемую и расширяемую кодовую базу. 
    Система формаций демонстрирует, как дополнительные функции могут быть интегрированы без 
    компромиссов в основной архитектуре.
    
    Весь код следует конвенциям C#, включает проверки на null и корректно освобождает ресурсы 
    для предотвращения утечек памяти на мобильных платформах.
  </Conclusion>

  <Screenshots>
    <Note>
      Скриншоты игры расположены в папке /screenshots/ и включают:
      - Главное меню с выбором формаций
      - Процесс боя с отображением счётчиков юнитов
      - Экран победы со статистикой
    </Note>
  </Screenshots>
</Readme>
