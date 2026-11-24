# Audit of the working rules against the session history

Date: 2026-11-25. Source: the user messages of 46 Claude Code sessions on CMiX, from 2026-08-19 to 2026-11-24, about 4400 unique messages. The sessions of `dl_performance_mode` and `CMiX_UI` are both included. Nothing in this file changes a rule. It reports what the history shows and what the rules do not cover yet.

## 1. What the owner asks for, by count

| Requests | Count | Rule today |
|---|---|---|
| Commit, push, merge, branch | 531 | CLAUDE.md, Commits |
| A name: find, change, check against a pattern | 286 | CLAUDE.md, Names |
| Explain simple, rephrase, shorter, step by step | 283 | CLAUDE.md, Language (partly) |
| Clean, simple, no bloat, DRY, standardize, reusable | 154 | agents only |
| Discuss first, do not touch, investigate and report | 104 | CLAUDE.md, Plans |
| How does Blender, Bitwig, TiXL, Notch, Unreal do it | 86 | none |
| Pro and cons, as a table | 62 | none |
| Show me some code | 60 | global CLAUDE.md (before and after) |
| The VL side is mine, C# only | 57 | CLAUDE.md, VL names (partly) |
| Search online, best practice, research | 54 | none |
| Before and after code | 48 | global CLAUDE.md |
| Honest answer, is it really the best | 33 | none |
| Optimize token usage | 28 | memory |
| No comments, remove comments | 27 | CLAUDE.md, Comments |
| What should I test or check now | 26 | none |
| Breaking change, old files | 16 | CLAUDE.md, Tests |
| One by one | 15 | memory |
| Flags, one-liners, tuples | 11 | agents only |
| No subagent | 6 | memory |
| No Co-Authored-By | 5 | both CLAUDE.md files |

Messages per month: August 668, September 1115, October 2132, November 475. The rules file was written in October and November. Most of the history is older than the rules, so a rule that already exists was often asked for before it existed. The table counts every ask, old or new.

## 2. The patterns

### 2.1 Short answers, one idea at a time

The most repeated correction of the whole history. The forms: "explain simple", "rephrase", "I don't understand", "too verbose", "step by step", "no analogy", "just tell me the name of the node", "one question at a time", "in short, what should I do". The owner reads better when the answer leads with the action, names the file or node first, and holds one decision.

The rule today: STE for comments, documentation and answers. STE makes sentences short. It does not limit the length of an answer, does not say one question at a time, and does not forbid analogies. The owner asked for no analogies on 2026-10-28 and 2026-11-23.

### 2.2 One recommendation, not a menu

"What is the best, honest", "which one is really better", "I'm not sure which to pick". When Claude gives options A, B, C, the owner asks which one and why. When the owner wants the options, they ask for a pro and cons table. A table is asked for 62 times.

No rule today.

### 2.3 Analysis and a before and after sample before code

"Before you do anything", "do not touch anything, investigate and report", "show me some code first", "I need to see a before and after before I approve the plan". The owner approves on the sample, then says go. The global CLAUDE.md asks for a before and after sample on a proposal. The asks continued after the rule was added on 2026-10-21, mostly for plans and audits. The rule names "a change to code" and the owner also wants it on each finding of an audit and each step of a plan.

The project rule "Analyze first and report the findings" covers the first half. It does not say that the main agent waits for a go before the first edit. The history shows the owner stopping an edit that started early ("wait you started working!!! there is no new branch", 2026-10-19).

### 2.4 The reference software

"How does Blender do it" is the most frequent reference, 40 times. Bitwig for modulation, TiXL and vvvv for node graphs, Notch and TouchDesigner for the operator view, Unreal and Stride for engine patterns, Atlassian for drag and drop. The owner wants to hear what they do before Claude proposes a design, and most UI decisions were taken by copying one of them (reset to default, context menu anchor, pointer away close, modifier stack, tab cycle).

No rule today. The agents `power-operator` and `venue-manager` know some of this, the main agent does not.

### 2.5 Search for the pattern, say its name

"Is there a known pattern", "search online for how this has been approached", "is this best practice", "what is the name of this pattern". The owner takes a decision more easily when the pattern has a name and one reference. Examples: visitor for the sync coordinator, ECS for the engine, service registry for the VL services, pairing dictionary for the composition relation.

No rule today.

### 2.6 Clean code, concrete rules

The owner's words: "no bloat", "I hate flags", "no one-liner", "no tuple", "constructors only assign", "no optional parameter that changes what a method does", "no comments, the name says it", "the same word in every layer", "no hardcoded value, use the token", "is it the cleanest". These rules are written in `cmix-step-executor.md` and `refactorer.md`. They are not in CLAUDE.md. The main agent and the `view-reviewer` agents do not see them. The executor also pins the branch `refactor/tree-structure`, which goes stale on the next branch.

The comments rule disagrees between files. CLAUDE.md: "Say what a piece of code does when it is not obvious." `refactorer.md`: "Comments explain why, never what." The owner, 27 times: no comment at all, a name instead. The executor says the same as the owner.

The interface rule disagrees too. CLAUDE.md: "A type gets an interface only when a test replaces it or when it hides Avalonia or the platform." `refactorer.md`: "Service: one capability behind an interface."

### 2.7 The VL side belongs to the owner

"VL doesn't matter, I'll do it", "focus on the C# side only, always", "engine stuff is on me", "do not search the C# side, VL has its own repository", "C# stays read only from VL" (decision of 2026-10-28), "the VL patch has the exact name of the C# class" (ControlFactory, 2026-08-28). Claude does the C#, reports what the VL side must change, and the owner patches. The owner pastes a patch as XML when a VL question needs Claude.

The rule today covers the names the patches use. It does not say who does the VL side, that VL only reads the C# objects, or that a new control class needs a VL patch of the same name.

### 2.8 A checklist after a UI step

"What should I check now", "tell me what to test", "give me a list of things to do and check", 26 times. The owner builds and tests in the Studio and reports back. Claude does not take screenshots (rule since 2026-10-21). The rule says "tell the owner what to look at". It does not say the form: a short list after each UI step.

### 2.9 Token use

"Optimize token usage" 28 times, mostly August to October. "Do not use subagents, it is slow" in September. In November the owner runs reviews and audits with agents and asks about the cost of `/code-review`. The memory says default Sonnet. CLAUDE.md says complexity 2 to 4 goes to Opus. The two rules differ. CLAUDE.md is the newer one.

### 2.10 Saved projects

September: "saved projects don't matter". 2026-10-26: "some users are using cmix, we can make breaking changes later". 2026-11-19: "no breaking change yet, I will at some point". CLAUDE.md now has the compatibility test rule. The memory `feedback_saved_projects_dont_matter` still says the opposite. It is wrong since 2026-10-26.

### 2.11 Words the owner refuses

The owner refused these words for CMiX: Kind, Service, Helper, Util, Manager for anything but the Core managers, Delete in code, Toast, Chrome, World, Scene, Editor as a suffix, Enabled where IsVisible exists, Additional, Resolver, Referencer, Holding, Sink, Pulse, Show for visibility. The owner's test for a name: "does a pattern in the project already use a word for this". CLAUDE.md lists Service, Helper, Util, Manager and Delete. It does not say the general rule: a word that CMiX does not use yet is a question for the owner, and the vocabulary of `ARCHITECTURE.md` section 4 wins.

### 2.12 Git

The pattern is stable: branch for a risky change, one commit per step, commit and push at the end of a step, CMiX first then the parent pointer, merge and delete the branch when done. Since 2026-11-23 the work is in `CMiX_UI` only and the parent repository gets no commit. CLAUDE.md and the memory cover this. The five asks for no Co-Authored-By are all older than the rule.

## 3. The files

### 3.1 `.claude/CLAUDE.md`

Covers well: language, commits, names and their order, the names that must not change, the Studio folders, the tests, the UI kit, the plans with complexity. Gaps, from the patterns above:

- Answers: length, one recommendation, one question at a time, no analogy, the file or node name first, a checklist after a UI step. (2.1, 2.2, 2.8)
- Approval: no edit before the owner says go on the analysis; a before and after sample on each step of a plan and each finding of an audit. (2.3)
- The reference software and the pattern name. (2.4, 2.5)
- The code rules that live only in the agent files: no flag parameter, no optional parameter that changes behaviour, constructors only assign, no tuple across a method, one-liners and switch expressions only when they read as plain text, no hardcoded size or color where a token exists. (2.6)
- The VL side: the owner patches it, Claude reports the change, VL reads and never writes the C# objects, a new control class needs a patch of the same name. (2.7)
- The vocabulary rule: a new word needs the owner. (2.11)

### 3.2 `~/.claude/CLAUDE.md` (global)

The before and after rule works. Its scope is "a change to code". The owner also wants it on a plan step and an audit finding. One line extends it.

### 3.3 The agents

- `cmix-step-executor.md`: pins one branch. Holds general rules that belong in CLAUDE.md. After the move, it can say "follow CLAUDE.md" and keep only the executor parts: read the step and nothing more, stop when the step does not fit, the build and test commands, CRLF and the Edit tool.
- `refactorer.md`: the comments rule and the service interface rule contradict CLAUDE.md. Points to `ARCHITECTURE.md` Part 2 as an unapproved proposal, which is still true.
- `view-reviewer-*.md`: read CLAUDE.md first, consistent. They would inherit the code rules once those are in CLAUDE.md.
- `color-system.md`, `power-operator.md`, `venue-manager.md`: used in the tree and node design sessions as intended. No conflict found.

### 3.4 `.claude/prompts/simplify-list-tree.md`

A one-off prompt for the session "ListTree code simplification" of 2026-11-24, now archived. It repeats the rules of CLAUDE.md. It can go, or move to `docs/plans` if the owner wants to keep the scope list.

### 3.5 The memory

- `feedback_saved_projects_dont_matter`: wrong since 2026-10-26, see 2.10. Updated in this audit.
- `feedback_token_management`: default Sonnet. CLAUDE.md gives complexity 2 to 4 to Opus. The memory should point at CLAUDE.md.
- `project_serversettings_split` (2026-08-25): the type was renamed on 2026-11-20 (`SyncSettings` to `SettingsSync`). To check and probably remove.
- `project_about_screen_credits` (2026-10-01): still open, the owner asked to be reminded.
- The rest matches the history.

### 3.6 `docs/plans`

`studio-services-split.md` says "Nothing in this file is built yet" and the session of 2026-11-24 built it. The line is stale. `keyboard-tree.md` is still a plan.

## 4. Proposed additions to `.claude/CLAUDE.md`

Each line is one decision of the owner. Nothing is applied.

```
## Answers

- Lead with the answer or the action. Name the file, the class or the node first.
- One recommendation and the reason. A list of options only when the owner asks,
  then as a table of pro and cons.
- One question at a time. Wait for the answer.
- No analogy. Use the words of CMiX.
- After a step that changes the UI, give a list of three to five things to look at
  in the Studio.

## Approval

- A plan, an audit or a proposal is a report. No edit before the owner says go.
- Each step of a plan and each finding of an audit carries a short before and
  after sample.
- For a UX decision, say first how Blender does it. For modulation, Bitwig.
  For a node graph, vvvv and TiXL. Then propose.
- For an architecture decision, name the pattern and one reference. Say when the
  simplest option is good enough.

## Code patterns (additions)

- No boolean flag parameter: two named methods. No optional parameter that
  changes what a method does.
- Constructors only assign fields. No tuple across a method boundary.
- A condition reads as plain text: an if block before a ternary, a switch
  expression or an expression-bodied one-liner.
- A size, a color or a margin comes from a token. A new hardcoded value is a
  finding.
- A new word that CMiX does not use yet is a question for the owner. The
  vocabulary of ARCHITECTURE.md section 4 wins.

## The engine side

- The owner patches the VL side. Claude changes the C# and reports what the
  patches must change: the names, the new types, the removed members.
- VL reads the C# objects and never writes them.
- A new control class needs a VL patch with the same name. Say so in the report.
```

## 5. Proposed cleanups

- Move the general rules out of `cmix-step-executor.md` into CLAUDE.md. Remove the branch name from the agent.
- Align `refactorer.md` with CLAUDE.md on comments and on interfaces.
- Remove `.claude/prompts/simplify-list-tree.md`.
- Fix the stale line of `docs/plans/studio-services-split.md`.
- Update the memory `feedback_token_management`, check `project_serversettings_split`.
