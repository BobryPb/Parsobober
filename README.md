## Parsobober

Parsobober is a static program analyzer for SIMPLE language. It is an interactive tool that automatically answers
PQL queries about the programs.

## Usage

### Run with DockerHub

```bash
curl https://raw.githubusercontent.com/BobryPb/Parsobober/main/tests/SPA-Official/cez/dropbox -o ./parsobober-code
docker run -i -v ./parsobober-code:/app/code maksimowiczm/parsobober
```

### Run with Docker build

```bash
git clone https://github.com/BobryPb/Parsobober parsobober
cd parsobober
docker build . -t parsobober
docker run -i -v ./tests/SPA-Official/cez/dropbox:/app/code parsobober
```

### Run with dotnet

```bash
git clone https://github.com/BobryPb/Parsobober parsobober
cd parsobober
dotnet run -c release --project src/Parsobober.Cli "./tests/SPA-Official/cez/dropbox"
```

## Project architecture

- `Parsobober.Cli` - Command line interface for the program
- `Parsobober.Pkb.*` - Program Knowledge Base
- `Parsobober.Simple.*`, `Parsobober.DesignExtractor` - SPA front-end 
- `Parsobober.Pql.*` - Query processor for PQL queries

## Relationships

### Calls

Relationship `Calls(p, q)` holds if procedure `p` directly calls `q`.

`Calls*(p,q)` holds if procedure `p` calls directly or indirectly `q`, that is:

### Modifies

Relationship Modifies is defined as follows:

- For an assignment `a`, `Modifies(a, v)` holds if variable `v` appears on the left hand side of `a`.
- For a container statement `s` (i.e., ‘if’ or ‘while’), `Modifies(s, v)` holds if there is a statement `s1` in the
  container such that `Modifies(s1, v)` holds.
- For a procedure `p`, `Modifies(p, v)` holds if there is a statement `s` in `p` or in a procedure called (directly or
  indirectly) from `p` such that `Modifies(s, v)` holds.
- For a procedure call statement `s` ‘call p’ `Modifies(s, v)` is defined in the same way as `Modifies(p, v)`.

### Uses

Relationship Uses is defined as follows:

- For an assignment `a`, `Uses(a, v)` holds if variable `v` appears on the right hand side of `a`.
- For a container statement `s` (i.e., ‘if’ or ‘while’), `Uses(s, v)` holds if there is a statement `s1` in the
  container such that `Uses(s1, v)` holds.
- For a procedure `p`, `Uses(p, v)` holds if there is a statement `s` in `p` or in a procedure called (directly or
  indirectly) from `p` such that `Uses(s, v)` holds.
- For a procedure call statement `s` ‘call p’ `Uses(s, v)` is defined in the same way as `Uses(p, v)`.

### Parent

For any two statements `s1` and `s2`, the relationship `Parent(s1, s2)` holds if `s2` is directly nested in `s1`.

For any two statements `s1` and `s2`, the relationship `Parent*(s1, s2)` holds if `s2` is nested in `s1` directly or
indirectly.

`Parent*(s1, s2)` holds if

- `Parent(s1, s2)` or
- `Parent(s1, s)` and `Parent*(s, s2)` for some statement `s`

### Follows

For any two statements `s1` and `s2`, the relationship `Follows(s1, s2)` holds if `s2` appears in program text directly
after `s1` at the same nesting level, and `s1` and `s2` belong to the same statement list.

For any two statements `s1` and `s2`, the relationship `Follows*(s1, s2)` holds if `s2` appears in program text after
`s1` at the same nesting level, and `s1` and `s2` belong to the same statement list, directly or indirectly.

`Follows*(s1, s2)` holds if

- `Follows(s1, s2)` or
- `Follows(s1, s)` and `Follows*(s, s2)` for some statement `s`

### Next

Let `n1` and `n2` be program lines.

Relationship `Next(n1, n2)` holds if `n1` and `n2` are in the same procedure, and `n2` can be executed immediately after
`n1` in some program execution sequence.

### Pattern

Black magic can only be possessed by sorcerers. [ not me :( ]

## Queries

- Calls
    - `Calls(<procedure name>, <procedure name>)`
    - `Calls(procedure, <procedure name>)`
    - `Calls(<procedure name>, procedure)`
    - `Calls(procedure, procedure)`
    - `Calls(<procedure name>, _)`
    - `Calls(_, <procedure name>)`
    - `Calls(_, _)`
- Calls transitive
    - `Calls*(<procedure name>, <procedure name>)`
    - `Calls*(procedure, <procedure name>)`
    - `Calls*(<procedure name>, procedure)`
    - `Calls*(procedure, procedure)`
    - `Calls*(<procedure name>, _)`
    - `Calls*(_, <procedure name>)`
    - `Calls*(_, _)`
- Modifies
    - `Modifies(<line number>, <variable name>)`
    - `Modifies(<procedure name>, <variable name>)`
    - `Modifies(<line number>, _)`
    - `Modifies(<procedure name>, _)`
    - `Modifies(statement, _)`
    - `Modifies(procedure, _)`
    - `Modifies(statement, <variable name>)`
    - `Modifies(<line number>, variable)`
    - `Modifies(<procedure name>, variable)`
    - `Modifies(procedure, <variable name>)`
    - `Modifies(statement, variable)`
- Uses
    - `Uses(<line number>, <variable name>)`
    - `Uses(<procedure name>, <variable name>)`
    - `Uses(<line number>, _)`
    - `Uses(<procedure name>, _)`
    - `Uses(statement, _)`
    - `Uses(procedure, _)`
    - `Uses(statement, <variable name>)`
    - `Uses(<line number>, variable)`
    - `Uses(<procedure name>, variable)`
    - `Uses(procedure, <variable name>)`
    - `Uses(statement, variable)`
- Parent
    - `Parent(<line number>, <line number>)`
    - `Parent(<line number>, _)`
    - `Parent(_, <line number>)`
    - `Parent(_, _)`
    - `Parent(statement, _)`
    - `Parent(_, statement)`
    - `Parent(statement, <line number>)`
    - `Parent(<line number>, statement)`
    - `Parent(statement, statement)`
- Parent transitive
    - `Parent*(<line number>, <line number>)`
    - `Parent*(<line number>, _)`
    - `Parent*(_, <line number>)`
    - `Parent*(_, _)`
    - `Parent*(statement, _)`
    - `Parent*(_, statement)`
    - `Parent*(statement, <line number>)`
    - `Parent*(<line number>, statement)`
    - `Parent*(statement, statement)`
- Follows
    - `Follows(<line number>, <line number>)`
    - `Follows(<line number>, _)`
    - `Follows(_, <line number>)`
    - `Follows(_, _)`
    - `Follows(statement, _)`
    - `Follows(_, statement)`
    - `Follows(statement, <line number>)`
    - `Follows(<line number>, statement)`
    - `Follows(statement, statement)`
- Follows transitive
    - `Follows*(<line number>, <line number>)`
    - `Follows*(<line number>, _)`
    - `Follows*(_, <line number>)`
    - `Follows*(_, _)`
    - `Follows*(statement, _)`
    - `Follows*(_, statement)`
    - `Follows*(statement, <line number>)`
    - `Follows*(<line number>, statement)`
    - `Follows*(statement, statement)`
- Next
    - `Next(<line number>, <line number>)`
    - `Next(<line number>, _)`
    - `Next(_, <line number>)`
    - `Next(_, _)`
    - `Next(program_line, _)`
    - `Next(_, program_line)`
    - `Next(program_line, <line number>)`
    - `Next(<line number>, program_line)`
- Next transitive
    - `Next*(<line number>, <line number>)`
    - `Next*(<line number>, _)`
    - `Next*(_, <line number>)`
    - `Next*(_, _)`
    - `Next*(program_line, _)`
    - `Next*(_, program_line)`
    - `Next*(program_line, <line number>)`
    - `Next*(<line number>, program_line)`
- With
    - `prodecure.procName` : `NAME`
    - `call.procName` : `NAME`
    - `variable.varName` : `NAME`
    - `constant.value` : `INTEGER`
    - `stmt.stmt#` : `INTEGER`
- Pattern

### Example queries using [example code](./tests/SPA-Official/cez/dropbox)

```
> if i, i1; 
> Select i such that Follows*(_, i1) and Follows*(i1, i)
34,55,166,170,173,230

> stmt s;
> Select s such that Parent(6,s) with s.stmt# = 7
7

> assign a;
> Select a pattern a(_, "width + incre + left")
7
```