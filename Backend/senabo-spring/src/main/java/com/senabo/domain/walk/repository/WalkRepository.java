package com.senabo.domain.walk.repository;

import com.senabo.domain.member.entity.Member;
import com.senabo.domain.walk.entity.Walk;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface WalkRepository extends JpaRepository<Walk, String>, WalkRepositoryCustom {

    List<Walk> deleteByMemberId(Member member);

}
