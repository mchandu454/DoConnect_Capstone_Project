// ─── Auth Types ────────────────────────────────────────────────────────────
export interface RegisterDto {
  username: string;
  password: string;
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userId: number;
  username: string;
  role: string;
}

// ─── Question Types ─────────────────────────────────────────────────────────
export interface CreateQuestionDto {
  title: string;
  description: string;
}

export interface QuestionResponse {
  questionId: number;
  userId: number;
  username: string;
  title: string;
  description: string;
  imagePath?: string;
  status: string;
  createdDate: string;
  answerCount: number;
}

export interface QuestionDetail extends QuestionResponse {
  answers: AnswerResponse[];
}

export interface PaginatedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

// ─── Answer Types ────────────────────────────────────────────────────────────
export interface CreateAnswerDto {
  questionId: number;
  answerText: string;
}

export interface AnswerResponse {
  answerId: number;
  questionId: number;
  questionTitle?: string;
  userId: number;
  username: string;
  answerText: string;
  imagePath?: string;
  status: string;
  createdDate: string;
}

// ─── Notification Type ───────────────────────────────────────────────────────
export interface Notification {
  id: string;
  message: string;
  type: 'question' | 'answer';
  timestamp: string;
  read: boolean;
}

