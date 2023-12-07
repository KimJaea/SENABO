package com.senabo.domain.member.repository;


import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.member.entity.Member;
import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import java.util.List;

import static com.senabo.domain.member.entity.QMember.member;


@Slf4j
@AllArgsConstructor
public class MemberRepositoryImpl implements MemberRepositoryCustom {
    private final JPAQueryFactory queryFactory;


    @Override
    public List<Member> findAllMemberNonComplete() {
        return queryFactory
                .selectFrom(member)
                .where(member.complete.isFalse())
                .fetch();
    }

    @Override
    public long updateStressLevel(String email, int score){
        return queryFactory
                .update(member)
                .set(member.stressLevel, score)
                .where(member.email.eq(email))
                .execute();
    }

    @Override
    public long updateAffection(String email, int score){
        return queryFactory
                .update(member)
                .set(member.affection, score)
                .where(member.email.eq(email))
                .execute();
    }


}
