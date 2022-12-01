export interface Level {
  strategic: Operational;
  tatic: Operational;
  operational: Operational;
}

export interface Operational {
  salaryMarkStructure: Structure;
  yourCompanyStructure: Structure;
}

export interface Structure {
  leadershipContributors: Contributor[];
  individualContributors: Contributor[];
}

export interface Contributor {
  id: number;
  level: null | string;
  code: null | string;
  active: boolean;
}

export interface SaveLevels {
  levels: LevelToSave[];
}

export interface LevelToSave {
  levelId: number;
  level: string;
  enabled: boolean;
}
