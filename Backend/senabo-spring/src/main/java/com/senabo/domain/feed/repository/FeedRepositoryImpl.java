package com.senabo.domain.feed.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.feed.entity.Feed;
import com.senabo.domain.member.entity.Member;
import lombok.RequiredArgsConstructor;
import org.springframework.data.jpa.repository.Query;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

import static com.senabo.domain.feed.entity.QFeed.feed;


@RequiredArgsConstructor
public class FeedRepositoryImpl implements FeedRepositoryCustom {

    private final JPAQueryFactory queryFactory;

    @Override
    public List<Feed> findByMemberId(Member member) {
        return queryFactory
                .selectFrom(feed)
                .where(feed.memberId.eq(member))
                .orderBy(feed.createTime.desc())
                .fetch();
    }

    @Override
    public Optional<Feed> findLatestData(Member member) {
        return Optional.ofNullable(queryFactory
                .selectFrom(feed)
                .where(feed.memberId.eq(member))
                .orderBy(feed.updateTime.desc())
                .limit(1)
                .fetchOne());
    }

    @Override
    public List<Feed> findFeedWeek(Member member, LocalDateTime endTime, LocalDateTime startTime) {
        return queryFactory
                .selectFrom(feed)
                .where(
                        feed.memberId.eq(member),
                        feed.updateTime.loe(endTime),
                        feed.createTime.goe(startTime)
                )
                .orderBy(feed.createTime.desc())
                .fetch();

    }
}
