package com.senabo.domain.feed.repository;

import com.senabo.domain.feed.entity.Feed;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface FeedRepository extends JpaRepository<Feed, String>, FeedRepositoryCustom{
    List<Feed> deleteByMemberId(Member member);

    Boolean existsByMemberId(Member member);

}
