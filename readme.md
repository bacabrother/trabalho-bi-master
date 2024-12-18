# OTIMIZADOR DE CRONOGRAMA DE POÇOS

#### Aluno: [Rômulo Luiz Pereira Ribeiro](https://github.com/bacabrother/)
#### Orientadora: [Nome Sobrenome](https://github.com/link_do_github).

---

Trabalho apresentado ao curso [BI MASTER](https://ica.puc-rio.ai/bi-master) como pré-requisito para conclusão de curso e obtenção de crédito na disciplina "Projetos de Sistemas Inteligentes de Apoio à Decisão".

- [Link para o código](https://github.com/bacabrother/trabalho-bi-master). 

---

### Resumo

O uso do algoritmo genético na elaboração de cronogramas de intervenção para poços, com múltiplas sondas e diversas restrições entre as atividades, mostrou-se altamente eficiente, gerando cronogramas que atendem rigorosamente todas as restrições estabelecidas.

Para testar o algoritmo, foram utilizados como dados de entrada 32 poços, totalizando 50 atividades, distribuídas entre 19 perfurações, 20 completações e 11 workovers. A alocação das atividades levou em consideração 3 sondas, com datas de disponibilidade, duração contratual e taxas diárias distintas entre elas. Além disso, foi levada em conta a relação de predecessores entre 14 atividades.

O desenvolvimento do algoritmo foi realizado utilizando o Unity 3D como plataforma, com a linguagem de programação C#. Para implementar o algoritmo genético, foi utilizado o plugin [Genetic Sharp](https://github.com/giacomelli/GeneticSharp) como base, mas os módulos de cromossomo, fitness, mutação, reinserção, seleção e terminação foram adaptados e criados especificamente para atender às necessidades do problema em questão. O módulo de crossover foi descartado, pois não se mostrou adequado para o tipo de problema abordado neste trabalho. Dessa forma, a solução foi customizada para otimizar o desempenho do algoritmo genético dentro do contexto específico de um cronograma com várias linhas de trabalho (sondas).

Os resultados indicam que o algoritmo genético é eficaz na criação de cronogramas otimizados, conseguindo resultados satisfatórios com um custo computacional relativamente baixo. Com um processamento de apenas 20 segundos e 1000 gerações, o algoritmo conseguiu gerar um cronograma cujo custo foi inferior ao do cronograma base, que foi criado manualmente. 

### 1. Introdução

#### 1.1 Construção de poços submarinos

Um cronograma de intervenção de poços tem como objetivo ordenar as intervenções de acordo com suas prioridades e restrições. Os principais tipos de intervenção são a perfuração, completação e workover. 

A alocação de um poço em um cronograma deve obedecer às seguintes restrições, sendo algumas obrigatórias e outras flexíveis:
* Ordem cronológica, predecessores naturais (Perfuração -> Completação -> Workover) -> Obrigatório
* Disponibilidade de recursos (Sonda, materiais para intervenção e frentes de serviço) -> Obrigatório
* Duração contratual de recursos (Sonda e frentes de serviços) -> Flexível
* Data máxima para uma intervenção (Evitar operações simultâneas, não se tornar o caminho crítico do projeto) -> Flexível
* Informações de outros poços, predecessores de oportunidade (Com os dados obtidos em uma intervenção pode-se otimizar a locação de outro poço) -> Flexível

A criação de um cronograma de intervenção de poços possui algumas particularidades:
* Várias sondas podem estar disponíveis para executar as intervenções.
* Uma intervenção de um poço não precisa ser seguida da outra, por exemplo, pode-se perfurar um poço e não o completar logo em seguida (perfuração quebrada). Porém, quando ocorre a “quebra” entre perfuração e completação, há um custo associado.
* Uma intervenção de um poço não precisa ser na mesma sonda que a intervenção passada, por exemplo, pode-se perfurar em uma sonda e completar em outra.

As restrições flexíveis possuem um custo associado quando não são seguidas. Dessa forma, o melhor cronograma será aquele com o menor custo onde todas as restrições foram atendidas.

#### 1.2 Criação manual de um cronograma de intervenção de poços

Para a criação manual de um cronograma, recomenda-se que os poços que possuem sucessores sejam alocados o mais rápido possível, de forma que seja possível seguir toda a rede de sucessores respeitando as folgas entre as atividades. Quando necessário, pode-se alocar uma atividade sem qualquer restrição entre duas atividades para que a folga entre uma atividade e outra seja cumprida. Além disso, é necessário observar a prioridade de cada atividade, a disponibilidade de recursos de cada atividade e a duração contratual de cada recurso.

#### 1.3 Algoritmo genético

O algoritmo genético é baseado na evolução dos indivíduos a cada geração. Ele começa com a seleção de indivíduos de uma geração para formar a próxima, por meio dos processos de cruzamento e mutação. Esses processos geram novos indivíduos, que são avaliados e comparados com os da geração anterior. Em seguida, os melhores indivíduos são selecionados para dar início a um novo ciclo evolutivo, gerando uma nova geração.

### 2. Modelagem

O desenvolvimento do algoritmo foi realizado utilizando o Unity 3D como plataforma, com a linguagem de programação C#. Para implementar o algoritmo genético, foi utilizado o plugin [Genetic Sharp](https://github.com/giacomelli/GeneticSharp).

#### 2.1 Entrada de dados

A entrada dos dados se da através de uma planilha do excel (*.xlsx), reunindo todos os dados necessários para o processamento. 

1) Dados da sonda:
* Nome da sonda
* Data de mobilização
* Duração máxima contratual
* Taxa diária

2) Dados dos poços:
* Nome do poço
* Tipo de atividade (perfuração/completação/workover)
* Duração da atividade

3) Tabela de predecessores:
* Nome do poço predecessor
* Nome do poço sucessor
* Tipo de atividade predecessora
* Tipo de atividade sucessora
* Folga entre término do predecessor e início do sucessor

Os dados de premissas são alimentados diretamente no Unity:

* Multa padrão (custo padrão de multa que será multiplicado por uma constante específica para cada restrição).
* Multa para cada predecessor obrigatório não atendido.
* Multa para cada dia de folga não atendida.
* Multa para cada recurso com tempo excedido além do tempo contratual.
* Multa para cada dia além da data máxima contratual.
* Multa para cada ocorrência de perfuração quebrada.
* Acréscimo de dias na perfuração quando com perfuração quebrada.
* Acréscimo de dias na completação quando com perfuração quebrada.

Além disso, existem fórmulas para multas que variam com os dias:

* Fórmula de multa para cada dia extra além da data máxima contratual = (1 + dias extras * multiplicador de diária) * taxa diária * dias extras
* Fórmula de multa para dia de folga não atendida = (1 - (folga real / folga recomendada)^1,2) * multa padrão * multiplicador para folga não atendida

#### 2.2 Modelagem do cronograma

O cronograma foi modelado através de 3 classes:
* Cronograma
* Sonda
* Atividade
  
Restrições considerados na modelagem:
* Data de disponibilidade de cada sonda.
* Duração contratual máxima para cada sonda.
* Taxa diária para cada sonda.
* Ordem cronológica para cada atividade (Perfuração antes da completação e completação antes do workover).
* Rede de predecessores entre atividades com folga mínima entre o término do predecessor e início do sucessor.
* Penalização para perfuração separada da completação.

Restrições não consideradas na modelagem:
* Prioridade de cada projeto.
* Prioridade de cada poço.
* Data máxima para conclusão dos poços de cada projeto.
* Necessidade de MPD para as intervenções.
* Benefícios da sonda dual.
* Duração contratual mínima de cada sonda.
* Disponibilidade de recursos necessários para cada atividade.
* Penalização da atividade inicial de cada sonda.
* Separação da perfuração em THD e BHD para economia de tempo.

##### 2.2.1 Cronograma

A classe cronograma possui uma lista de Sondas.

Dados de estatísticas, úteis para a avaliação e comparação dos cronogramas:
* Custo
* Quantidade de predecessores obrigatórios descumpridos.
* Quantidade de folgas entre predecessores descumpridas.
* Quantidade de perfurações quebradas.
* Quantidade de sondas com dias além da data máxima contratual.

Um cronograma perfeito é aquele onde não existe ocorrências de predecessores obrigatórios descumpridos, onde todas as folgas de predecessores são atendidas, não existem perfurações quebradas e nenhuma sonda possui dias além da data máxima contratual. O custo desse cronograma será mínimo, porém, como as taxas diárias de cada sonda são diferentes, um cronograma perfeito pode ter um custo diferente de outro cronograma perfeito.

Métodos:
Criação/Geração:
* Criação totalmente aleatória.
* Criação sorteando atividades de um conjunto de atividades que não possuem predecessores ou restrições.
* Criação sorteando atividades de um conjunto de atividades que não possuem predecessores mas que possuem sucessores, e sorteando atividades que não possuem predecessores e sucessores quando não for possível alocar uma atividade do 1º conjunto.

Avaliação do custo final do cronograma:
* Verificação de perfuração quebrada
* Processamento das novas durações (referente as perfurações quebradas)
* Cálculo do custo final (com todas as multas)

Genes:
*Converter cronograma para lista de genes
*Criar cronograma através de uma lista de genes

##### 2.2.2 Sonda

A classe sonda possui uma lista de atividades.

Dados de entrada com as caractéristicas da sonda.
Dados de estatísticas:
* Custo.
* Duração total.
* Total de dias extras.

Métodos:
* Alocação de atividades.
* Processamento das novas durações (referente as perfurações quebradas)
* Cálculo do custo final (incluindo apenas a multa de data após a data máxima contratual)

##### 2.2.3 Atividade

Dados de entrada com as características da sonda.
Dados que são preenchidos conforme a atividade é alocada em uma sonda:
* Nome da sonda alocada
* Ordem da atividade na sonda
* Data de início
* Data de término
* Duração real

Dado sobre a qualidade da alocação da atividade (bad position), o valor padrão desse campo é 1 e aumenta conforme o não atendimento das restrições:
* 100 para as atividades de um poço com predecessores obrigatórios descumpridos.
* 60 para as atividades de perfuração e completação do poço com perfuração quebrada.
* 50 para a última atividade da sonda com atividades além da data máxima contratual.
* 30 para a predecessora e sucessora quando a folga mínima não é respeitada.

Os métodos da atividade são:
* Alocar atividade
* Reprocessar datas
* Calcular multa de predecessores
* Verificar perfuração quebrada

#### 2.3 Algoritmo Genético

Foi utilizado o plugin [Genetic Sharp](https://github.com/giacomelli/GeneticSharp) como base para o algoritmo genético, mas os módulos de cromossomo, fitness, mutação, reinserção, seleção e terminação foram adaptados e criados especificamente para atender às necessidades do problema em questão. O módulo de crossover foi descartado, pois não se mostrou adequado para o tipo de problema abordado neste trabalho.

##### 2.3.1 Cromossomo

O cromossomo possui um cronograma vinculado, e através desse cronograma utilizando o método "Converter cronograma para lista de genes", são geradas duas listas de genes, sendo uma de atividades e outra de sondas.

*A lista de genes das atividades é uma lista ordenada que indica qual posição será ocupada por cada atividade da lista de atividades carregadas no input, essa lista é uma lista de índices.
*A lista de genes das sondas indica os cortes na lista de genes das atividades, que corresponderão a cada sonda carregada no input.

Por exemplo:

Lista de atividades carregadas no input: [Atv0, Atv1, Atv2, Atv3, Atv4, Atv5, Atv6, Atv7, Atv8, Atv9]
Lista de sondas carregadas no input: [Sonda1, Sonda2, Sonda3]

Lista de genes de atividades: [4, 6, 2, 3, 1, 0, 7, 9, 8, 5]
Lista de genes de sondas: [3, 8]

Índice das atividades da Sonda1: [4, 6, 2]
Índice das atividades da Sonda2: [3, 1, 0, 7, 9]
Índice das atividades da Sonda3: [8, 5]

Atividades da Sonda1: [Atv4, Atv6, Atv2]
Atividades da Sonda1: [Atv3, Atv1, Atv0, Atv7, Atv9]
Atividades da Sonda1: [Atv8, Atv5]

Ao criar um novo cromossomo, é criado um novo cronograma e esse novo cronograma é convertido em genes. 

##### 2.3.2 Fitness

O Fitness é simplesmente o custo esperado de um cronograma dividido pelo custo real do cronograma. O custo aproximado considera a soma das durações das atividades multiplicada pela média de taxa diária das sondas.

##### 2.3.3 Crossover

O módulo de crossover foi descartado, pois não se mostrou adequado para o tipo de problema abordado neste trabalho. Visto que a lista de genes inclui atividades de todas as sondas, dessa forma, o cruzamento de dados apenas atrapalha o processo de evolução.

##### 2.3.4 Mutação

A mutação ocorre entre 2 genes a cada geração.
O primeiro gene escolhido considera o peso do campo "bad position" de cada atividade, dessa forma, atividades com valores elevados de bad position possuem maior chance de serem selecionadas (ver item 2.2.3).
O segundo gene escolhido é aleatório, sem considerar pesos.

##### 2.3.5 Reinserção

A reinserção une os melhores indivíduos da última geração, todos os indivíduos que sofreram mutação e uma quantidade determinada de novos indivíduos.

##### 2.3.6 Seleção

A seleção é o início de uma nova geração, selecionando os melhores indivíduos da geração anterior e um número pequeno de indivíduos aleatórios.

#### 2.3.7 Terminação

A evolução será interrompida sempre que um cronograma perfeito for criado (ver item 2.2.1) ou quando um número determinado de gerações for atingido.

#### 2.4 Exportação dos melhores cronogramas

Ao término da simulação, os melhores cronogramas podem ser exportados para uma nova planilha no Excel para serem analisados em um dashboard no Power BI.

### 3. Resultados

Para testar o algoritmo, foram utilizados como dados de entrada 32 poços, totalizando 50 atividades, distribuídas entre 19 perfurações, 20 completações e 11 workovers. A alocação das atividades levou em consideração 3 sondas, com datas de disponibilidade, duração contratual e taxas diárias distintas entre elas. Além disso, foi levada em conta a relação de predecessores entre 14 atividades.

Para comparação, foi criado um cronograma base manualmente, atendendo todas as restrições e com um custo inicial de referência (R$ 291.585.000) para comparação com os cronogramas gerados automaticamente.

Ao rodar o algoritmo genérico, foi obtido cronogramas com custos inferiores ao de referência, sendo o melhor deles (R$ 290.065.000), em um tempo de processamento de 20 segundos e 1000 gerações.


### 4. Conclusões

Os resultados indicam que o algoritmo genético é eficaz na criação de cronogramas otimizados, conseguindo resultados satisfatórios com um custo computacional relativamente baixo. Com um processamento de apenas 20 segundos e 1000 gerações, o algoritmo conseguiu gerar um cronograma cujo custo foi inferior ao do cronograma base, que foi criado manualmente. Dessa forma, a modelagem foi satisfatória, proporcionando soluções de boa qualidade em um pequeno tempo, contribuindo para a redução do esforço humano necessário para a elaboração do cronograma. Essa eficiência pode ser útil em cenários onde ajustes dinâmicos são necessários.

---

Matrícula: 221.101.016

Pontifícia Universidade Católica do Rio de Janeiro

Curso de Pós Graduação *Business Intelligence Master*
