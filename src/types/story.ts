export interface Story {
  id: number;
  userId: number;
  username: string;
  imageUrl: string;
  caption?: string;
  createdAt: string;
  expiresAt: string;
  viewCount: number;
  hasViewed: boolean;
}

export interface CreateStoryDto {
  imageUrl: string;
  caption?: string;
}

export interface StoryView {
  id: number;
  storyId: number;
  userId: number;
  username: string;
  viewedAt: string;
}
