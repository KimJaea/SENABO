package com.senabo.domain.communication.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.communication.entity.Communication;
import com.senabo.domain.member.entity.Member;
import lombok.RequiredArgsConstructor;

import java.time.LocalDateTime;
import java.util.List;

import static com.senabo.domain.communication.entity.QCommunication.communication;

@RequiredArgsConstructor
public class CommunicationRepositoryImpl implements CommunicationRepositoryCustom {
    private final JPAQueryFactory queryFactory;

    @Override
    public List<Communication> findByMemberId(Member member) {
        return queryFactory
                .selectFrom(communication)
                .where(communication.memberId.eq(member))
                .orderBy(communication.createTime.desc())
                .fetch();
    }

    @Override
    public List<Communication> findCommunicationWeek(Member member, LocalDateTime endTime, LocalDateTime startTime) {
        return queryFactory
                .selectFrom(communication)
                .where(
                        communication.memberId.eq(member),
                        communication.updateTime.loe(endTime),
                        communication.createTime.goe(startTime)
                )
                .orderBy(communication.createTime.desc())
                .fetch();
    }

    @Override
    public Long countCommunicationWeek(Member member, LocalDateTime lastStart) {
        return queryFactory
                .select(communication.count())
                .from(communication)
                .where(
                        communication.memberId.eq(member),
                        communication.createTime.goe(lastStart)
                )
                .fetchOne();
    }
}
