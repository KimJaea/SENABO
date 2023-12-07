package com.senabo.domain.bath.repository;

import com.senabo.domain.bath.entity.Bath;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;


public interface BathRepositoryCustom{
    List<Bath> findByMemberId(Member memberId);
}
